using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Framework
{
    internal sealed class QueryPack
    {
        private static readonly string[] aliases = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        private IDictionary<Type, QueryTable> pairs;
        private StringBuilder builder;

        public bool HasConditions { get; set; } = false;

        public QueryState State { get; set; }

        public IList<string> SelectFields { get; }

        public QueryPack()
        {
            this.builder = new StringBuilder();
            this.pairs = new Dictionary<Type, QueryTable>();

            this.SelectFields = new List<string>();
        }

        public void Append(string value)
        {
            builder.Append(value);
        }

        public void AppendFormat(string format, params object[] args)
        {
            builder.AppendFormat(format, args);
        }

        public bool Contains(Type type)
        {
            return pairs.Keys.Contains(type);
        }

        public QueryTable GetTable(Type type)
        {
            if (!pairs.Keys.Contains(type))
            {
                pairs[type] = new QueryTable(DbEntityCache.GetEntityName(type), aliases[pairs.Keys.Count]);
            }

            return pairs[type];
        }

        public ICollection<QueryTable> GetTables()
        {
            return pairs.Values;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
