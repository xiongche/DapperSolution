using System.Data;

namespace Project.Framework
{
    public sealed class LogBulkRecord
    {
        public string TableName { get; set; }

        public ActionType Type { get; set; }

        public DataTable Table { get; set; }

        public LogBulkRecord(string tableName, DataTable table, ActionType type)
        {
            this.TableName = tableName;
            this.Table = table;
            this.Type = type;
        }
    }
}
