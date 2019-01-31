namespace Project.Framework
{
    public sealed class LogRecord
    {
        public string TableName
        { get; set; }

        public object Model
        { get; set; }

        public ActionType Type
        { get; set; }

        public LogRecord(string tableName, object model, ActionType type)
        {
            this.TableName = tableName;
            this.Model = model;
            this.Type = type;
        }
    }

    public enum ActionType
    {
        Insert,
        Update,
        Delete
    }
}
