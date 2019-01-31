using System;

namespace Project.Framework
{
    [Serializable]
    public sealed class DbEntity
    {
        public string Schema { get; set; }

        public string Name { get; set; }

        public DbEntityType Type { get; set; } = DbEntityType.Table;

        public string FullName
        {
            get
            {
                return string.Format("{0}.{1}", this.Schema, this.Name);
            }
        }

        public string PrimaryKeys
        { get; set; }

        public bool IsNewSequentialPrimaryKey
        { get; set; }

        public DbEntity()
        {
            this.Schema = "dbo";
            this.IsNewSequentialPrimaryKey = false;
        }
    }

    public enum DbEntityType
    {
        Table = 1,
        View = 2
    }
}
