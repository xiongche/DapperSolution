namespace Project.Framework
{
    public sealed class BulkCommandText
    {
        public string TableCommandText { get; set; }

        public string ExecutionCommandText { get; set; }

        public BulkCommandText(string table, string execution)
        {
            this.TableCommandText = table;
            this.ExecutionCommandText = execution;
        }
    }
}
