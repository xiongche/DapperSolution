using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Project.Framework
{
    public sealed class HashValueHelper
    {
        public static bool HasChanged<T>(SqlConnection conn, SqlTransaction tran, T item) where T : class, IDbQuery, IDbModel, new()
        {
            var hashValue = GetHashValue<T>(conn, tran, item);
            return !StringHelper.Compare(item.HashValue, hashValue);
        }

        public static string Generate<T>(T item) where T : class, IDbQuery
        {
            var builder = new StringBuilder();

            var helpers = PropertyHelper.GetProperties(item).Where(helper => !DbConst.AuditProperties.Contains(helper.Name));
            helpers.Each(helper => builder.Append(GetValue(helper, item)));

            return EncryptHelper.EncryptByMd5(builder.ToString());
        }

        private static string GetHashValue<T>(SqlConnection conn, SqlTransaction tran, T item) where T : class, IDbQuery, new()
        {
            var key = $"{typeof(T)}.GetItem.QueryHashValue";
            var sql = CommandTextCache.GetCachedCommandText(key);
            if (string.IsNullOrEmpty(sql))
            {
                sql = CommandTextCache.GetHashValueQueryCommandText(key, DbEntityCache.GetDbEntity<T>());
            }

            var param = GetPrimaryKeyParams<T>(item);
            return param.Keys.Count == 0 ? string.Empty : conn.ExecuteScalar<string>(sql, param, tran, null, CommandType.Text);
        }

        private static string GetValue(PropertyHelper helper, object item)
        {
            var value = helper.GetValue(item);
            return value == null ? string.Empty : value.ToString();
        }

        private static IDictionary<string, object> GetPrimaryKeyParams<T>(T item) where T : class, IDbQuery, new()
        {
            var pairs = new Dictionary<string, object>();
            var primaryKeys = DbEntityCache.GetDbEntity<T>().PrimaryKeys.Split(',');

            var helpers = PropertyHelper.GetProperties(item);
            helpers.Where(helper => primaryKeys.Contains(helper.Name)).Each(helper => pairs[helper.Name] = helper.GetValue(item));

            return pairs;
        }

        //public static void BulkCompare<T>(SqlConnection conn, SqlTransaction tran, DataTable table) where T : class, IDbQuery, new()
        //{
        //    var primaryKeys = DbTableCache.GetDbTable<T>().PrimaryKeys.Split(',');

        //    var view = new DataView(table);
        //    var keyTable = view.ToTable(true, primaryKeys);
        //    var hashTable = GetHashTable<T>(conn, tran, keyTable);

        //    foreach (DataRow hashRow in hashTable.Rows)
        //    {
        //        var builder = new StringBuilder();
        //        builder.Append(string.Join(" AND ", primaryKeys.Select(key => $"{key}='{hashRow[key].ToString()}'")));
        //        builder.AppendFormat(" AND HashValue='{0}'", hashRow["HashValue"].ToString());

        //        var rows = table.Select(builder.ToString());
        //        if (rows.Length > 0)
        //        {
        //            table.Rows.Remove(rows[0]);
        //        }
        //    }
        //}

        //private static DataTable GetHashTable<T>(SqlConnection conn, SqlTransaction tran, DataTable table) where T : class, IDbQuery, new()
        //{
        //    var tableKey = $"{typeof(T)}.BulkUpdate.HashTable";
        //    var queryKey = $"{typeof(T)}.BulkUpdate.QueryHashTable";

        //    var tableCommandText = CommandTextCache.GetCachedCommandText(tableKey);
        //    var queryCommandText = CommandTextCache.GetCachedCommandText(queryKey);

        //    if (string.IsNullOrEmpty(tableCommandText) || string.IsNullOrEmpty(queryCommandText))
        //    {
        //        tableCommandText = CommandTextCache.GetBuildTableCommandText(tableKey, GetHashTableColumns<T>());
        //        queryCommandText = CommandTextCache.GetHashTableQueryCommandText(queryKey, DbTableCache.GetDbTable<T>());
        //    }

        //    var tableName = string.Format("#HashTable{0}", DateTimeHelper.GetTimestamp());
        //    tableCommandText = tableCommandText.Replace("@TableName", tableName);
        //    Execute(conn, tran, tableCommandText);

        //    WriteToServer(conn, tran, tableName, table);

        //    queryCommandText = queryCommandText.Replace("@TableName", tableName);
        //    return ExecuteTable(conn, tran, queryCommandText);
        //}

        //private static void WriteToServer(SqlConnection conn, SqlTransaction tran, string tableName, DataTable table)
        //{
        //    var bulk = tran == null ? new SqlBulkCopy(conn) : new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);

        //    bulk.DestinationTableName = tableName;
        //    foreach (DataColumn column in table.Columns)
        //    {
        //        bulk.ColumnMappings.Add(new SqlBulkCopyColumnMapping(column.ColumnName, column.ColumnName));
        //    }
        //}

        //private static void Execute(SqlConnection conn, SqlTransaction tran, string commandText)
        //{
        //    var command = tran == null ? new SqlCommand(commandText, conn) : new SqlCommand(commandText, conn, tran);
        //    command.ExecuteNonQuery();
        //}

        //private static DataTable ExecuteTable(SqlConnection conn, SqlTransaction tran, string commandText)
        //{
        //    var command = tran == null ? new SqlCommand(commandText, conn) : new SqlCommand(commandText, conn, tran);
        //    var reader = command.ExecuteReader();

        //    var table = new DataTable();
        //    table.Load(reader);

        //    return table;
        //}

        //private static IList<DbColumn> GetHashTableColumns<T>() where T : class, IDbQuery, new()
        //{
        //    var dbTable = DbTableCache.GetDbTable<T>();

        //    var columns = dbTable.PrimaryKeys.Split(',').SelectList(key => new DbColumn(key, "UNIQUEIDENTIFIER"));
        //    columns.Add(new DbColumn("HashValue", "VARCHAR", "32"));

        //    return columns;
        //}
    }
}
