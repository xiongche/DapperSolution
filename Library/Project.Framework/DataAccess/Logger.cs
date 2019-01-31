using Dapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Project.Framework
{
    public sealed class Logger : ILoggable
    {
        private static ConcurrentDictionary<string, string> textCache = new ConcurrentDictionary<string, string>();
        private const string SQL_COLUMN = "SELECT C.name AS NAME FROM sys.tables AS T INNER JOIN sys.columns AS C ON T.object_id=C.object_id WHERE T.name=@Name AND C.name!='LogId' ORDER BY C.column_id";
        private const string SQL_TABLE = "SELECT COUNT(1) AS NAME FROM sys.tables AS T WHERE T.name=@Name";

        private string instance;
        private IList<LogRecord> records;
        private IList<LogBulkRecord> bulkRecords;

        public Logger(string instance)
        {
            this.instance = instance;

            records = new List<LogRecord>();
            bulkRecords = new List<LogBulkRecord>();
        }

        public void Add<T>(T item, ActionType type) where T : class, IDbEntity, IDbModel, new()
        {
            if (item != null)
            {
                this.records.Add(new LogRecord(GetTableName<T>(), item, type));
            }
        }

        public void Add<T>(DataTable table, ActionType type) where T : class, IDbEntity, IDbModel, new()
        {
            if (table.Rows.Count > 0)
            {
                bulkRecords.Add(new LogBulkRecord(GetTableName<T>(), table, type));
            }
        }

        public void Clear()
        {
            this.records.Clear();
            this.bulkRecords.Clear();
        }

        public void Record()
        {
            using (var conn = DbHelper.GetConnection(instance))
            {
                records.Each(record => Save(conn, record));
                bulkRecords.Each(record => BulkSave(conn, record));

                Clear();
            }
        }

        private void Save(SqlConnection conn, LogRecord record)
        {
            if (!IsTableExist(conn, record.TableName))
            {
                return;
            }

            var sql = GetCommandText(conn, record.TableName);
            if (!string.IsNullOrEmpty(sql))
            {
                sql = SetAuditInfo(sql, record.Type, (IDbModel)record.Model);
                conn.Execute(sql, record.Model, null, null, CommandType.Text);
            }
        }

        private void BulkSave(SqlConnection conn, LogBulkRecord record)
        {
            if (!IsTableExist(conn, record.TableName))
            {
                return;
            }

            ProcessBulkRecord(record);

            var bulk = new SqlBulkCopy(conn);

            bulk.DestinationTableName = record.TableName;
            foreach (DataColumn column in record.Table.Columns)
            {
                bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
            }

            bulk.WriteToServer(record.Table);
        }

        private void ProcessBulkRecord(LogBulkRecord record)
        {
            if (record.Type == ActionType.Insert)
            {
                record.Table.Columns["CreatedBy"].ColumnName = "LoggedBy";
                record.Table.Columns["CreatedDate"].ColumnName = "LoggedDate";
            }
            if (record.Type == ActionType.Update || record.Type == ActionType.Delete)
            {
                record.Table.Columns["UpdatedBy"].ColumnName = "LoggedBy";
                record.Table.Columns["UpdatedDate"].ColumnName = "LoggedDate";
            }

            SetActionType(record.Table, record.Type);
        }

        private void SetActionType(DataTable table, ActionType type)
        {
            var actionType = GetActionTypeName(type);

            table.Columns.Add("ActionType", typeof(string));
            foreach (DataRow row in table.Rows)
            {
                row["ActionType"] = actionType;
            }
        }

        private string GetTableName<T>() where T : class, IDbEntity, IDbModel, new()
        {
            return string.Format("{0}_Log", DbEntityCache.GetDbEntity<T>().Name);
        }

        private string GetCommandText(SqlConnection conn, string tableName)
        {
            if (!textCache.ContainsKey(tableName))
            {
                textCache[tableName] = BuildCommandText(conn, tableName);
            }

            return textCache[tableName];
        }

        private string BuildCommandText(SqlConnection conn, string tableName)
        {
            var columnNames = GetColumNames(conn, tableName);
            if (columnNames.Count() == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            builder.AppendFormat("INSERT INTO {0} (", tableName);
            builder.Append(string.Join(",", columnNames.Select(columnName => string.Format("[{0}]", columnName))));
            builder.Append(") VALUES (");
            builder.Append(string.Join(",", columnNames.Select(columnName => string.Format("@{0}", columnName))));
            builder.Append(");");

            return builder.ToString();
        }

        private string SetAuditInfo(string commandText, ActionType type, IDbModel iDbModel)
        {
            commandText = commandText.Replace("@IsActive", type == ActionType.Delete ? "0" : "1");
            commandText = commandText.Replace("@ActionType", string.Format("'{0}'", GetActionTypeName(type)));

            var loggedBy = type == ActionType.Insert ? iDbModel.CreatedBy : iDbModel.UpdatedBy.Value;
            commandText = commandText.Replace("@LoggedBy", string.Format("'{0}'", loggedBy.ToString()));

            var loggedDate = type == ActionType.Insert ? iDbModel.CreatedDate : iDbModel.UpdatedDate.Value;
            commandText = commandText.Replace("@LoggedDate", string.Format("'{0}'", loggedDate.ToString("yyyy-MM-dd HH:mm:ss.fff")));

            return commandText;
        }

        private string GetActionTypeName(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Insert:
                    return "C";
                case ActionType.Update:
                    return "U";
                case ActionType.Delete:
                    return "D";
                default:
                    throw new ArithmeticException("actionType");
            }
        }

        private bool IsTableExist(SqlConnection conn, string tableName)
        {
            return conn.QueryFirst<int>(SQL_TABLE, new { Name = tableName }, null, null, CommandType.Text) > 0;
        }

        private IEnumerable<string> GetColumNames(SqlConnection conn, string tableName)
        {
            return conn.Query<string>(SQL_COLUMN, new { Name = tableName }, null, true, null, CommandType.Text);
        }
    }
}
