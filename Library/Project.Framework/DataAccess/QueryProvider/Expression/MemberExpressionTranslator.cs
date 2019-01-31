using System;
using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class MemberExpressionTranslator
    {
        public static void Translate(MemberExpression expression, QueryPack pack)
        {
            if (pack.State == QueryState.Select)
            {
                Select(expression, pack);
            }
            else if (pack.State == QueryState.Where || pack.State == QueryState.Join || pack.State == QueryState.OrderBy || pack.State == QueryState.GroupBy)
            {
                InnerTranslate(expression, pack);
            }
        }

        private static void Select(MemberExpression expression, QueryPack pack)
        {
            var table = pack.GetTable(expression.Member.DeclaringType);
            pack.SelectFields.Add($"{table.Alias}.{expression.Member.Name}");
        }

        private static void InnerTranslate(MemberExpression expression, QueryPack pack)
        {
            if (pack.Contains(expression.Expression.Type))
            {
                var table = pack.GetTable(expression.Expression.Type);
                pack.Append($"{table.Alias}.{expression.Member.Name}");
            }
            else
            {
                var name = expression.Member.Name;
                pack.Append($"@{name[0].ToString().ToUpperInvariant()}{name.Substring(1, name.Length - 1)}");
            }
        }
    }
}
