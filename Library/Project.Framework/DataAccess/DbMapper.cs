using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Project.Framework
{
    public sealed class DbMapper : IDbMapper
    {
        private SqlConnection conn;
        private SqlTransaction tran;

        public DbMapper(SqlConnection conn)
        {
            this.conn = conn;
        }

        public void SetTran(SqlTransaction tran)
        {
            this.tran = tran;
        }

        public void Insert<T>(T item) where T : class, IDbEntity, IDbModel, new()
        {
            ThrowIfNull(item);

            var key = string.Format("{0}.Insert", typeof(T));
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>(item);
                var fields = ColumnField.GetFields(item);

                sql = CommandTextCache.GetInsertCommandText(key, dbEntity, fields);
            }

            conn.Execute(sql, item, tran, null, CommandType.Text);
        }

        public void Update<T>(T item) where T : class, IDbEntity, IDbModel, new()
        {
            ThrowIfNull(item);

            var key = string.Format("{0}.Update", typeof(T));
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>(item);
                var fields = ColumnField.GetFields(item);

                sql = CommandTextCache.GetUpdateCommandText(key, dbEntity, fields);
            }

            conn.Execute(sql, item, tran, null, CommandType.Text);
        }

        public void Delete<T>(object param, IDbModel dbModel) where T : class, IDbEntity, IDbModel, new()
        {
            ThrowIfNull(param);

            var key = string.Format("{0}.Delete", typeof(T));
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                sql = CommandTextCache.GetDeleteCommandText(key, dbEntity);
            }

            sql = sql.Replace("@UpdatedBy", dbModel.UpdatedBy.Value.ToString());
            sql = sql.Replace("@UpdatedDate", dbModel.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            conn.Execute(sql, param, tran, null, CommandType.Text);
        }

        public void BulkInsert<T>(DataTable table) where T : class, IDbEntity, IDbModel, new()
        {
            WriteToServer(DbEntityCache.GetEntityName<T>(), table);
        }

        public void BulkUpdate<T>(DataTable table) where T : class, IDbEntity, IDbModel, new()
        {
            var tableKey = string.Format("{0}.BulkTable", typeof(T));
            var executionKey = string.Format("{0}.BulkUpdate", typeof(T));
            var commandText = GetBulkUpdateCommandText(typeof(T), tableKey, executionKey);

            BulkExecute(table, commandText);
        }

        public void BulkDelete<T>(DataTable table) where T : class, IDbEntity, IDbModel, new()
        {
            var tableKey = string.Format("{0}.BulkTable", typeof(T));
            var executionKey = string.Format("{0}.BulkDelete", typeof(T));
            var commandText = GetBulkDeleteCommandText(typeof(T), tableKey, executionKey);

            BulkExecute(table, commandText);
        }

        public T GetItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            ThrowIfNull(param);
            return QueryItem<T>($"{typeof(T)}.GetItem.Param.{by}", param, conditions, true);
        }

        public T CheckItem<T>(string by, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            ThrowIfNull(param);
            return QueryItem<T>($"{typeof(T)}.CheckItem.Param.{by}", param, conditions, false);
        }

        public R GetValue<T, R>(string by, string column, object param, string[] conditions) where T : class, IDbQuery, new()
        {
            ThrowIfNull(param);

            var key = string.Format("{0}.GetValue.Param.{1}", typeof(T), by);
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                var fields = ConvertHelper.ToKeys(param);
                sql = CommandTextCache.GetValueCommandText(key, dbEntity, column, fields, conditions);
            }

            return conn.ExecuteScalar<R>(sql, param, tran, null, CommandType.Text);
        }

        public IList<T> GetTop<T>(string by, int count, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new()
        {
            ThrowIfNull(param);

            var format = count == 1 ? "{0}.GetTop.Param.{1}" : "{0}.GetTops.Param.{1}";
            var key = string.Format(format, typeof(T), by);
            var sql = CommandTextCache.GetCachedCommandText(key);

            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                var fields = ConvertHelper.ToKeys(param);
                sql = CommandTextCache.GetTopQueryCommandText(key, dbEntity, count, fields, conditions, orderBy);
            }

            return conn.Query<T>(sql, param, tran, true, null, CommandType.Text).ToList();
        }

        public IList<R> GetValues<T, R>(string by, string field, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new()
        {
            var key = string.Format("{0}.GetValues.Param.{1}", typeof(T), by);
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                var fields = param == null ? new string[] { } : ConvertHelper.ToKeys(param);
                sql = CommandTextCache.GetValuesCommandText(key, dbEntity, field, fields, conditions, orderBy);
            }

            return conn.Query<R>(sql, param, tran, true, null, CommandType.Text).ToList();
        }

        public IList<T> GetItems<T>(string by, object param, string[] conditions, string orderBy) where T : class, IDbQuery, new()
        {
            return QueryItems<T>($"{typeof(T)}.GetItems.Param.{by}", param, conditions, orderBy, true);
        }

        public IList<T> GetItems<T>(string column, object param, DataTable table, string[] conditions, string orderBy) where T : class, IDbQuery, new()
        {
            return QueryItems<T>(column, param, table, conditions, orderBy, true);
        }

        public R GetValue<R>(object param, DataTable table, string sql)
        {
            if (table != null)
            {
                WriteToServer(table);
            }

            return conn.ExecuteScalar<R>(sql, param, tran, null, CommandType.Text);
        }

        public T GetItem<T>(object param, DataTable table, string sql)
        {
            if (table != null)
            {
                WriteToServer(table);
            }

            return conn.QueryFirstOrDefault<T>(sql, param, tran, null, CommandType.Text);
        }

        public IList<T> GetItems<T>(object param, DataTable table, string sql)
        {
            if (table != null)
            {
                WriteToServer(table);
            }

            return conn.Query<T>(sql, param, tran, true, null, CommandType.Text).ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void ThrowIfNull(object param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("Object is null");
            }
        }

        private void Execute(string cmdText)
        {
            var cmd = tran == null ? new SqlCommand(cmdText, conn) : new SqlCommand(cmdText, conn, tran);
            cmd.ExecuteNonQuery();
        }

        private void BulkExecute(DataTable table, BulkCommandText commandText)
        {
            var tableName = string.Format("#Temp{0}", DateTimeHelper.GetTimestamp());
            commandText.TableCommandText = commandText.TableCommandText.Replace("@TableName", tableName);
            Execute(commandText.TableCommandText);

            WriteToServer(tableName, table);

            commandText.ExecutionCommandText = commandText.ExecutionCommandText.Replace("@TableName", tableName);
            Execute(commandText.ExecutionCommandText);
        }

        private void WriteToServer(string tableName, DataTable table)
        {
            var bulk = tran == null ? new SqlBulkCopy(conn) : new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);

            bulk.DestinationTableName = tableName;
            foreach (DataColumn column in table.Columns)
            {
                bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
            }

            //table.TableName = DateTimeHelper.GetTimestamp();
            bulk.WriteToServer(table);
        }

        private string WriteToServer(DataTable table)
        {
            var tableName = table.TableName.StartsWith("#") ? table.TableName : string.Format("#{0}", table.TableName);
            var tableCommandText = CommandTextCache.GetSplitTableCommandText(tableName, table);

            Execute(tableCommandText);
            WriteToServer(tableName, table);

            return tableName;
        }

        private IEnumerable<DbColumn> GetDbColumns(string tableName)
        {
            return conn.Query<DbColumn>(DbColumn.SQL_COLUMN_DEFINITION, new { TableName = tableName }, tran, true, null, CommandType.Text);
        }

        private BulkCommandText GetBulkUpdateCommandText(Type type, string tableKey, string executionKey)
        {
            var tableCommandText = CommandTextCache.GetCachedCommandText(tableKey);
            var executionCommandText = CommandTextCache.GetCachedCommandText(executionKey);
            var commandText = new BulkCommandText(tableCommandText, executionCommandText);

            if (string.IsNullOrEmpty(tableCommandText) || string.IsNullOrEmpty(executionCommandText))
            {
                var dbEntity = DbEntityCache.GetDbEntity(type);
                var dbColumns = GetDbColumns(dbEntity.Name).Where(dbColumn => !DbConst.BulkUpdateExcludedColumns.Contains(dbColumn.Name));

                commandText.TableCommandText = CommandTextCache.GetBulkTableCommandText(tableKey, dbColumns);
                commandText.ExecutionCommandText = CommandTextCache.GetBulkUpdateCommandText(executionKey, dbEntity, dbColumns);
            }

            return commandText;
        }

        private BulkCommandText GetBulkDeleteCommandText(Type type, string tableKey, string executionKey)
        {
            var tableCommandText = CommandTextCache.GetCachedCommandText(tableKey);
            var executionCommandText = CommandTextCache.GetCachedCommandText(executionKey);
            var commandText = new BulkCommandText(tableCommandText, executionCommandText);

            if (string.IsNullOrEmpty(tableCommandText) || string.IsNullOrEmpty(executionCommandText))
            {
                var dbEntity = DbEntityCache.GetDbEntity(type);
                var dbColumns = GetDbColumns(dbEntity.Name).Where(dbColumn => !DbConst.BulkUpdateExcludedColumns.Contains(dbColumn.Name));

                commandText.TableCommandText = CommandTextCache.GetBulkTableCommandText(tableKey, dbColumns);
                commandText.ExecutionCommandText = CommandTextCache.GetBulkDeleteCommandText(executionKey, dbEntity);
            }

            return commandText;
        }

        private T QueryItem<T>(string key, object param, string[] conditions, bool activeOnly) where T : class, IDbQuery, new()
        {
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                var pairs = param as IDictionary<string, object>;
                var fields = pairs == null ? ConvertHelper.ToKeys(param) : pairs.Keys;

                sql = CommandTextCache.GetSingleQueryCommandText(key, dbEntity, fields, conditions, activeOnly);
            }

            return conn.QueryFirstOrDefault<T>(sql, param, tran, null, CommandType.Text);
        }

        private IList<T> QueryItems<T>(string key, object param, string[] conditions, string orderBy, bool activeOnly) where T : class, IDbQuery, new()
        {
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                var dbEntity = DbEntityCache.GetDbEntity<T>();
                var fields = param == null ? new ColumnField[] { } : ColumnField.GetFields(param);
                sql = CommandTextCache.GetMultiQueryCommandText(key, dbEntity, fields, conditions, orderBy, activeOnly);
            }

            return conn.Query<T>(sql, param, tran, true, null, CommandType.Text).ToList();
        }

        private IList<T> QueryItems<T>(string column, object param, DataTable table, string[] conditions, string orderBy, bool activeOnly) where T : class, IDbQuery, new()
        {
            var dbEntity = DbEntityCache.GetDbEntity<T>();
            var fields = param == null ? new ColumnField[] { } : ColumnField.GetFields(param);
            var tableName = WriteToServer(table);
            var sql = CommandTextCache.GetSplitQueryCommandText(tableName, column, dbEntity, fields, conditions, orderBy, activeOnly);

            return conn.Query<T>(sql, param, tran, true, null, CommandType.Text).ToList();
        }
    }
}
