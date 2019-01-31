namespace System.Text
{

    [Serializable]
    public sealed class ScriptBuilder
    {
        private StringBuilder builder;

        public ScriptBuilder()
        {
            builder = new StringBuilder();
        }

        public ScriptBuilder(string value) : this()
        {
            Append(value);
        }

        public ScriptBuilder(string format, params object[] args) : this()
        {
            Append(format, args);
        }

        public ScriptBuilder Append(string value)
        {
            builder.AppendFormat("{0} ", value);
            return this;
        }

        public ScriptBuilder Append(string format, params object[] args)
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
