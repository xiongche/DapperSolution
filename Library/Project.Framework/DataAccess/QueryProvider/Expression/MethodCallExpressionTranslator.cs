using System;
using System.Linq;
using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class MethodCallExpressionTranslator
    {
        public static void Translate(MethodCallExpression expression, QueryPack pack)
        {
            var methodName = expression.Method.Name;
            if (IsLikeMethod(methodName))
            {
                TranslateLikeMethod(methodName, expression, pack);
            }
            if (IsReplaceMethod(methodName))
            {
                TranslateReplace(methodName, expression, pack);
            }
            if (IsUpperMethod(methodName))
            {
                TranslateToUpper(methodName, expression, pack);
            }
        }

        private static bool IsLikeMethod(string methodName)
        {
            return StringHelper.Compare(methodName, "Contains") || StringHelper.Compare(methodName, "StartsWith") || StringHelper.Compare(methodName, "EndsWith");
        }

        private static bool IsReplaceMethod(string methodName)
        {
            return StringHelper.Compare(methodName, "Replace");
        }

        private static bool IsUpperMethod(string methodName)
        {
            return StringHelper.Compare(methodName, "ToUpper") || StringHelper.Compare(methodName, "ToUpperInvariant");
        }

        private static void TranslateLikeMethod(string methodName, MethodCallExpression expression, QueryPack pack)
        {
            ExpressionTranslator.Translate(expression.Object, pack);
            pack.Append(" LIKE ");

            if (StringHelper.Compare(methodName, "Contains") || StringHelper.Compare(methodName, "EndsWith"))
            {
                pack.Append("'%'+");
            }

            ExpressionTranslator.Translate(expression.Arguments[0], pack);

            if (StringHelper.Compare(methodName, "Contains") || StringHelper.Compare(methodName, "StartsWith"))
            {
                pack.Append("+'%'");
            }
        }

        private static void TranslateReplace(string methodName, MethodCallExpression expression, QueryPack pack)
        {
            pack.Append("REPLACE(");
            ExpressionTranslator.Translate(expression.Object, pack);

            for (int i = 0, j = 2; i < j; i++)
            {
                pack.Append(", ");
                ExpressionTranslator.Translate(expression.Arguments[i], pack);
            }
            pack.Append(")");
        }
        
        private static void TranslateToUpper(string methodName, MethodCallExpression expression, QueryPack pack)
        {
            pack.Append("UPPER(");
            ExpressionTranslator.Translate(expression.Object, pack);
            pack.Append(")");
        }
    }
}
