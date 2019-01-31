namespace Project.Framework
{
    public interface ISqlBuilder
    {
        ISqlBuilder Append(string value);

        ISqlBuilder Append(string format, params object[] args);

        string ToString();
    }
}
