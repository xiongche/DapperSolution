using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class BinaryExpressionTranslator
    {
        public static void Translate(BinaryExpression expression, QueryPack pack)
        {
            if (pack.State == QueryState.Where)
            {
                Where(expression, pack);
            }
            else if (pack.State == QueryState.Join)
            {
                Join(expression, pack);
            }
        }

        private static void Where(BinaryExpression expression, QueryPack pack)
        {
            ExpressionTranslator.Translate(expression.Left, pack);
            ExpressionTranslator.Translate(expression.NodeType, pack, IsNull(expression.Right));
            ExpressionTranslator.Translate(expression.Right, pack);
        }

        private static void Join(BinaryExpression expression, QueryPack pack)
        {
            ExpressionTranslator.Translate(expression.Left, pack);
            ExpressionTranslator.Translate(expression.NodeType, pack, false);
            ExpressionTranslator.Translate(expression.Right, pack);
        }

        private static bool IsNull(Expression expression)
        {
            var constantExpression = expression as ConstantExpression;

            return constantExpression != null ? constantExpression.Value == null : expression == null;
        }
    }
}
