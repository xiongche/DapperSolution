using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class NewExpressionTranslator
    {
        public static void Translate(NewExpression expression, QueryPack pack)
        {
            if (pack.State == QueryState.Select)
            {
                Select(expression, pack);
            }
        }

        private static void Select(NewExpression expression, QueryPack pack)
        {
            foreach (var argument in expression.Arguments)
            {
                ExpressionTranslator.Translate(argument as MemberExpression, pack);
            }
        }
    }
}
