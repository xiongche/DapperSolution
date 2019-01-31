using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Project.Framework
{
    internal sealed class CommandTextCache
    {
        private static ConcurrentDictionary<string, string> textCache = new ConcurrentDictionary<string, string>();

        internal static string GetInsertCommandText(string key, DbEntity dbEntity, IEnumerable<ColumnField> fields)
        {
            ThrowIfNoColumns(fields);

            var columns = fields.SelectList(field => field.Name);
            columns.Add("IsActive");
            DbConst.UpdateProperties.Each(column => columns.Remove(column));

            var builder = new StringBuilder();

            builder.Append(string.Format("INSERT INTO {0}.{1} (", dbEntity.Schema, dbEntity.Name));
            builder.Append(string.Join(",", columns.Select(column => string.Format("[{0}]", column))));
            builder.Append(") VALUES (");
            builder.Append(string.Join(",", columns.Select(column => column == "IsActive" ? "1" : string.Format("@{0}", column))));
            builder.Append(");");

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetUpdateCommandText(string key, DbEntity dbEntity, IEnumerable<ColumnField> fields)
        {
            ThrowIfNoColumns(fields);

            var columns = fields.SelectList(field => field.Name);
            columns.Add("IsActive");
            DbConst.InsertProperties.Each(column => columns.Remove(column));

            var primaryKeys = dbEntity.PrimaryKeys.Split(',');
            primaryKeys.Each(primaryKey => columns.Remove(primaryKey));

            var builder = new StringBuilder(string.Format("UPDATE {0}.{1} SET ", dbEntity.Schema, dbEntity.Name));

            builder.Append(string.Join(",", columns.Select(column => string.Format("[{0}]=@{0}", column))).Replace("@IsActive", "1"));
            builder.Append(" WHERE ");
            builder.Append(string.Join(" AND ", primaryKeys.Select(primaryKey => string.Format("[{0}]=@{0}", primaryKey))));

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetDeleteCommandText(string key, DbEntity dbEntity)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("UPDATE {0}.{1} SET IsActive=0,", dbEntity.Schema, dbEntity.Name);
            builder.Append("UpdatedBy='@UpdatedBy',");
            builder.Append("UpdatedDate='@UpdatedDate' ");
            builder.Append("WHERE ");

            var primaryKeys = dbEntity.PrimaryKeys.Split(',');
            builder.Append(string.Join(" AND ", primaryKeys.Select(primaryKey => string.Format("[{0}]=@{0}", primaryKey))));

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetValueCommandText(string key, DbEntity dbEntity, string column, IEnumerable<string> fields, string[] conditions)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT {0} FROM {1}.{2} ", column, dbEntity.Schema, dbEntity.Name);
            builder.Append(dbEntity.Type == DbEntityType.Table ? "WHERE IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND {condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => string.Format("AND [{0}]=@{0}", field))));
            }

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetValuesCommandText(string key, DbEntity dbEntity, string column, IEnumerable<string> fields, string[] conditions, string orderBy)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT {0} FROM {1}.{2} ", column, dbEntity.Schema, dbEntity.Name);
            builder.Append(dbEntity.Type == DbEntityType.Table ? "WHERE IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND {condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => string.Format("AND [{0}]=@{0}", field))));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append(string.Format(" ORDER BY {0}", orderBy));
            }

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetSingleQueryCommandText(string key, DbEntity dbEntity, IEnumerable<string> fields, string[] conditions, bool activeOnly)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT * FROM {0}.{1} ", dbEntity.Schema, dbEntity.Name);
            builder.Append(dbEntity.Type == DbEntityType.Table && activeOnly ? "WHERE IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND {condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => string.Format("AND [{0}]=@{0}", field))));
            }

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetTopQueryCommandText(string key, DbEntity dbEntity, int count, IEnumerable<string> fields, string[] conditions, string orderBy)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT TOP({0}) * FROM {1}.{2} ", count, dbEntity.Schema, dbEntity.Name);
            builder.Append(dbEntity.Type == DbEntityType.Table ? "WHERE IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND {condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => $"AND [{field}]=@{field}")));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append(string.Format(" ORDER BY {0}", orderBy));
            }

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetMultiQueryCommandText(string key, DbEntity dbEntity, IEnumerable<ColumnField> fields, string[] conditions, string orderBy, bool activeOnly)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT * FROM {0}.{1} ", dbEntity.Schema, dbEntity.Name);
            builder.Append(dbEntity.Type == DbEntityType.Table && activeOnly ? "WHERE IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND {condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => $"AND { BuildMultiQueryCondition(field)}")));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append(string.Format(" ORDER BY {0}", orderBy));
            }

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetHashValueQueryCommandText(string key, DbEntity dbEntity)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT HashValue FROM {0}.{1} WITH(NOLOCK) WHERE IsActive=1 ", dbEntity.Schema, dbEntity.Name);
            builder.Append(string.Join(" ", dbEntity.PrimaryKeys.Split(',').Select(primaryKey => $"AND {primaryKey}=@{primaryKey}")));

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetCachedCommandText(string key)
        {
            return textCache.ContainsKey(key) ? textCache[key] : null;
        }

        internal static string GetBulkTableCommandText(string key, IEnumerable<DbColumn> dbColumns)
        {
            var builder = new StringBuilder();
            builder.Append("IF object_id(N'tempdb..@TableName') IS NOT NULL ");
            builder.Append("DROP TABLE @TableName; ");
            builder.Append("CREATE TABLE @TableName( ");

            builder.Append(string.Join(",", dbColumns.Select(dbColumn => dbColumn.GetColumnDefinition())));
            builder.Append(" )");

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetBulkUpdateCommandText(string key, DbEntity dbEntity, IEnumerable<DbColumn> dbColumns)
        {
            var builder = new StringBuilder();
            builder.Append("UPDATE A SET ");

            var primaryKeys = dbEntity.PrimaryKeys.Split(',');
            var columnNames = dbColumns.Where(dbColumn => !primaryKeys.Contains(dbColumn.Name)).Select(dbColumn => dbColumn.Name);
            builder.Append(string.Join(",", columnNames.Select(columnName => string.Format("A.[{0}]=T.[{0}]", columnName))));

            builder.AppendFormat(" FROM {0} AS A ", dbEntity.FullName);
            builder.Append(" INNER JOIN @TableName AS T ON ");
            builder.Append(string.Join(" AND ", primaryKeys.Select(primaryKey => string.Format("A.[{0}]=T.[{0}]", primaryKey))));
            builder.Append(";DROP TABLE @TableName;");

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetBulkDeleteCommandText(string key, DbEntity dbEntity)
        {
            var builder = new StringBuilder();
            builder.Append("UPDATE A SET A.IsActive=0 ");
            builder.Append(",A.UpdatedBy=T.UpdatedBy ");
            builder.Append(",A.UpdatedDate=T.UpdatedDate ");

            var primaryKeys = dbEntity.PrimaryKeys.Split(',');

            builder.AppendFormat(" FROM {0} AS A ", dbEntity.FullName);
            builder.Append(" INNER JOIN @TableName AS T ON ");
            builder.Append(string.Join(" AND ", primaryKeys.Select(primaryKey => string.Format("A.[{0}]=T.[{0}]", primaryKey))));
            builder.Append(";DROP TABLE @TableName;");

            return CacheCommandText(key, builder.ToString());
        }

        internal static string GetSplitTableCommandText(string tableName, DataTable table)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("IF object_id(N'tempdb..{0}') IS NOT NULL ", tableName);
            builder.AppendFormat("DROP TABLE {0}; ", tableName);
            builder.AppendFormat("CREATE TABLE {0}(Item {1} PRIMARY KEY); ", tableName, GetColumnDbType(table.Columns["Item"]));

            return builder.ToString();
        }

        internal static string GetSplitQueryCommandText(string tableName, string column, DbEntity dbEntity, IEnumerable<ColumnField> fields, string[] conditions, string orderBy, bool activeOnly)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("SELECT T.* FROM {0} AS T ", dbEntity.FullName);
            builder.AppendFormat("INNER JOIN {0} AS S ", tableName);
            builder.AppendFormat("ON T.{0}=S.Item ", column);
            builder.AppendFormat(dbEntity.Type == DbEntityType.Table && activeOnly ? "WHERE T.IsActive=1 " : "WHERE 1=1 ");

            if (conditions != null && conditions.Length > 0)
            {
                builder.Append(string.Join(" ", conditions.Select(condition => $"AND T.{condition}")));
            }
            else
            {
                builder.Append(string.Join(" ", fields.Select(field => $"AND { BuildMultiQueryCondition("T", field)}")));
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                builder.Append(string.Format(" ORDER BY T.{0}", orderBy));
            }

            builder.AppendFormat(";DROP TABLE {0};", tableName);

            return builder.ToString();
        } 

        private static void ThrowIfNoColumns(IEnumerable<ColumnField> fields)
        {
            if (fields.Count() == 0)
            {
                throw new ArgumentNullException("no fields");
            }
        }

        private static string CacheCommandText(string key, string sql)
        {
            textCache[key] = sql;

            return textCache[key];
        }

        private static string BuildMultiQueryCondition(ColumnField field)
        {
            return field.IsNullable ? $"(@{field.Name} IS NULL OR [{field.Name}]=@{field.Name})" : $"[{field.Name}]=@{field.Name}";
        }

        private static string BuildMultiQueryCondition(string alias, ColumnField field)
        {
            return field.IsNullable ? $"(@{field.Name} IS NULL OR {alias}.[{field.Name}]=@{field.Name})" : $"{alias}.[{field.Name}]=@{field.Name}";
        }

        private static string GetColumnDbType(DataColumn column)
        {
            if (column.DataType == typeof(int))
            {
                return "INT";
            }
            if (column.DataType == typeof(Guid))
            {
                return "UNIQUEIDENTIFIER";
            }

            return "VARCHAR(128)";
        }
    }
}
