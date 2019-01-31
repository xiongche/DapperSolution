using System;
using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class ConstantExpressionTranslator
    {
        internal static void Translate(ConstantExpression expression, QueryPack pack)
        {
            if (pack.State == QueryState.Where || pack.State == QueryState.Join)
            {
                InnerTranslate(expression, pack);
            }
        }

        private static void InnerTranslate(ConstantExpression expression, QueryPack pack)
        {
            if (expression == null || expression.Value == null)
            {
                pack.Append("NULL");
            }
            else if (expression.Type == typeof(string) || expression.Type == typeof(Guid))
            {
                pack.AppendFormat("N'{0}' ", expression.Value.ToString());
            }
            else if (expression.Type == typeof(bool))
            {
                pack.Append((bool)expression.Value ? "1" : "0");
            }
            else
            {
                pack.AppendFormat("{0} ", expression.Value.ToString());
            }
        }
    }
}
