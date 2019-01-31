using System;
using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class ExpressionTranslator
    {
        public static void Translate(Expression expression, QueryPack pack)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression can not be null.");
            }

            if (expression is ConstantExpression)
            {
                ConstantExpressionTranslator.Translate(expression as ConstantExpression, pack);
            }
            else if (expression is BinaryExpression)
            {
                BinaryExpressionTranslator.Translate(expression as BinaryExpression, pack);
            }
            else if (expression is MemberExpression)
            {
                MemberExpressionTranslator.Translate(expression as MemberExpression, pack);
            }
            else if (expression is MethodCallExpression)
            {
                MethodCallExpressionTranslator.Translate(expression as MethodCallExpression, pack);
            }
            else if (expression is NewArrayExpression)
            {

            }
            else if (expression is NewExpression)
            {
                NewExpressionTranslator.Translate(expression as NewExpression, pack);
            }
            else if (expression is UnaryExpression)
            {
                UnaryExpressionTranslator.Translate(expression as UnaryExpression, pack);
            }
            else if (expression is MemberInitExpression)
            {

            }
            else if (expression is ListInitExpression)
            {

            }
            else
            {
                throw new NotImplementedException("Unknow expression");
            }
        }

        public static void Translate(ExpressionType nodeType, QueryPack pack, bool isNull)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    pack.Append(" AND ");
                    break;
                case ExpressionType.Equal:
                    pack.Append(isNull ? " IS " : "=");
                    break;
                case ExpressionType.GreaterThan:
                    pack.Append(">");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    pack.Append(">=");
                    break;
                case ExpressionType.NotEqual:
                    pack.Append(isNull ? " IS NOT " : "!=");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    pack.Append(" OR ");
                    break;
                case ExpressionType.LessThan:
                    pack.Append("<");
                    break;
                case ExpressionType.LessThanOrEqual:
                    pack.Append("<=");
                    break;
                case ExpressionType.Add:
                    pack.Append("+");
                    break;
                case ExpressionType.Subtract:
                    pack.Append("-");
                    break;
                case ExpressionType.Multiply:
                    pack.Append("*");
                    break;
                case ExpressionType.Divide:
                    pack.Append("/");
                    break;
                case ExpressionType.Modulo:
                    pack.Append("%");
                    break;
                default:
                    throw new NotImplementedException("Unkonw type");
            }
        }
    }
}
