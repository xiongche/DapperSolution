using System.Collections.Generic;

namespace System.Data
{
    public static class DataSetExtension
    {
        public static DataTable ToTable(this DataSet dataSet, int tableIndex)
        {
            return dataSet.Tables[tableIndex];
        }

        public static int RowCount(this DataSet dataSet, int tableIndex)
        {
            return dataSet.Tables[tableIndex].Rows.Count;
        }

        public static int ToInt(this DataSet dataSet, int tableIndex)
        {
            return ConvertHelper.ToInt(dataSet.Tables[tableIndex].Rows[0][0]);
        }

        public static IEnumerable<DataRow> ToEnumeration(this DataSet dataSet, int tableIndex)
        {
            return dataSet.Tables[tableIndex].AsEnumerable();
        }
    }
}
