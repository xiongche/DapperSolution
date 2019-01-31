namespace Project.Framework
{
    internal sealed class QueryTable
    {
        public string Name { get; set; }

        public string Alias { get; set; }

        public QueryTable(string name, string alias)
        {
            this.Name = name;
            this.Alias = alias;
        }
    }
}
