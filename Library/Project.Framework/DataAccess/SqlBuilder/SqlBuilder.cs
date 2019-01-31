using Project.Framework;

namespace System.Text
{
    public sealed class SQL
    {
        public static ISqlBuilder Append()
        {
            return new SqlBuilder();
        }

        public static ISqlBuilder Append(string value)
        {
            return new SqlBuilder(value);
        }

        public static ISqlBuilder Append(string format, params object[] args)
        {
            return new SqlBuilder(format, args);
        }
    }

    public sealed class SqlBuilder : ISqlBuilder
    {
        private StringBuilder builder;

        public SqlBuilder()
        {
            builder = new StringBuilder();
        }

        public SqlBuilder(string value) : this()
        {
            Append(value);
        }

        public SqlBuilder(string format, params object[] args) : this()
        {
            Append(format, args);
        }

        public ISqlBuilder Append(string value)
        {
            builder.AppendFormat("{0} ", value);
            return this;
        }

        public ISqlBuilder Append(string format, params object[] args)
        {
            builder.AppendFormat(format, args);
            builder.Append(" ");

            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
