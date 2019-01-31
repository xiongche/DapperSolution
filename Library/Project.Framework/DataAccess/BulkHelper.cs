using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Project.Framework
{
    internal sealed class BulkHelper<T> where T : class, IDbQuery, IDbModel, new()
    {
        internal static DataTable ConvertToInsertTable(IEnumerable<T> items)
        {
            var columns = BulkColumnCache<T>.GetColumns();
            var pairs = columns.Where(column => !DbConst.UpdateProperties.Contains(column.Key));

            var table = NewTable(pairs);
            items.Each(item => table.Rows.Add(NewRow(table, pairs, item)));

            return table;
        }

        internal static DataTable ConvertToUpdateTable(IEnumerable<T> items)
        {
            var columns = BulkColumnCache<T>.GetColumns();
            var pairs = columns.Where(column => !DbConst.InsertProperties.Contains(column.Key));

            var table = NewTable(pairs);
            items.Each(item => table.Rows.Add(NewRow(table, pairs, item)));          

            return table;
        }

        internal static DataTable ConvertToDeleteTable(IEnumerable<T> items)
        {
            var columns = BulkColumnCache<T>.GetColumns();
            var pairs = columns.Where(column => !DbConst.InsertProperties.Contains(column.Key));

            var table = NewTable(pairs);
            items.Each(item => table.Rows.Add(NewDeleteRow(table, pairs, item)));

            return table;
        }

        private static DataTable NewTable(IEnumerable<KeyValuePair<string, Type>> columns)
        {
            var table = new DataTable();
            columns.Each(column => table.Columns.Add(column.Key, column.Value));

            return table;
        }

        private static DataRow NewRow(DataTable table, IEnumerable<KeyValuePair<string, Type>> columns, T item)
        {
            var row = table.NewRow();
            row["IsActive"] = true;

            var pairs = GetPairs(item);
            var keys = columns.Where(column => column.Key != "IsActive").Select(column => column.Key);
            keys.Each(key => row[key] = pairs[key] == null ? DBNull.Value : pairs[key]);

            return row;
        }

        private static DataRow NewDeleteRow(DataTable table, IEnumerable<KeyValuePair<string, Type>> columns, T item)
        {
            var row = NewRow(table, columns, item);
            row["IsActive"] = false;

            return row;
        }

        private static IDictionary<string, object> GetPairs(object value)
        {
            var pairs = new Dictionary<string, object>();
            PropertyHelper.GetProperties(value).Each(helper => pairs[helper.Name] = helper.GetValue(value));

            return pairs;
        }
    }
}
