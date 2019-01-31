using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Project.BusinessLogic
{
    internal sealed class ScriptHelper
    {
        internal const string SPLIT_TABLE = "#SplitTemp";

        internal static DataTable ToTable<T>(IEnumerable<T> items)
        {
            return ToTable<T>(SPLIT_TABLE, items);
        }

        internal static DataTable ToTable<T>(string tableName, IEnumerable<T> items)
        {
            var table = new DataTable();
            table.TableName = tableName.StartsWith("#") ? tableName.TrimStart('#') : tableName;
            table.Columns.Add("Item", typeof(T));

            items.Distinct().Each(item => table.Rows.Add(NewRow<T>(table, item)));

            return table;
        }

        private static DataRow NewRow<T>(DataTable table, T item)
        {
            var row = table.NewRow();
            row["Item"] = item;

            return row;
        }
    }
}
