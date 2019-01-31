using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class UnaryExpressionTranslator
    {
        public static void Translate(UnaryExpression expression, QueryPack pack)
        {
            Select(expression, pack);
        }

        private static void Select(UnaryExpression expression, QueryPack pack)
        {
            ExpressionTranslator.Translate(expression.Operand, pack);
        }
    }
}
