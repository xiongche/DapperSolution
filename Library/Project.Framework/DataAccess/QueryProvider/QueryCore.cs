using System;
using System.Linq.Expressions;

namespace Project.Framework
{
    public sealed class QueryCore<T>
    {
        private QueryTranslator translator;

        public QueryCore()
        {
            this.translator = new QueryTranslator();
        }

        public QueryCore<T> Top(int count, Type type, LambdaExpression expression = null)
        {
            translator.Add(QuerySegment.GetCount(count, type, SegmentState.Top, expression));

            return this;
        }

        public QueryCore<T> Count(bool distinct, Type type, LambdaExpression expression = null)
        {
            translator.Add(QuerySegment.GetDistinct(distinct, type, SegmentState.Count, expression));

            return this;
        }

        public QueryCore<T> Function(SegmentState state, Type type, LambdaExpression expression = null)
        {
            translator.Add(new QuerySegment(type, state, expression));

            return this;
        }

        public QueryCore<T> Select(bool distinct, Type type, LambdaExpression expression = null)
        {
            translator.Add(QuerySegment.GetDistinct(distinct, type, SegmentState.Select, expression));

            return this;
        }

        public QueryCore<T> WithNoLock()
        {
            translator.Add(QuerySegment.GetSqlText("WITH(NOLOCK)"));
            return this;
        }

        public QueryCore<T> InnerJoin<T1>(Expression<Func<T, T1, bool>> expression)
        {
            return InnerJoin<T, T1>(expression);
        }

        public QueryCore<T> InnerJoin<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T2), SegmentState.InnerJoin, expression));

            return this;
        }

        public QueryCore<T> InnerJoinTempTable(string tableName, Expression<Func<T, object>> expression)
        {
            return InnerJoinTempTable<T>(tableName, expression);
        }

        public QueryCore<T> InnerJoinTempTable<T1>(string tableName, Expression<Func<T1, object>> expression)
        {
            translator.Add(QuerySegment.GetTempTable(tableName, typeof(T1), SegmentState.InnerJoinTempTable, expression));

            return this;
        }

        public QueryCore<T> LeftJoin<T1>(Expression<Func<T, T1, bool>> expression)
        {
            return LeftJoin<T, T1>(expression);
        }

        public QueryCore<T> LeftJoin<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T2), SegmentState.LeftJoin, expression));

            return this;
        }

        public QueryCore<T> RightJoin<T1>(Expression<Func<T, T1, bool>> expression)
        {
            return RightJoin<T, T1>(expression);
        }

        public QueryCore<T> RightJoin<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T2), SegmentState.RightJoin, expression));

            return this;
        }

        public QueryCore<T> Where(Expression<Func<T, bool>> expression)
        {
            return Where<T>(expression);
        }

        public QueryCore<T> Where<T1>(Expression<Func<T1, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.Where, expression));

            return this;
        }

        public QueryCore<T> WhereActive(Expression<Func<T, bool>> expression = null)
        {
            return WhereActive<T>(expression);
        }

        public QueryCore<T> WhereActive<T1>(Expression<Func<T1, bool>> expression = null)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.WhereActive, expression));

            return this;
        }

        public QueryCore<T> AndActive(Expression<Func<T, bool>> expression = null)
        {
            return AndActive<T>(expression);
        }

        public QueryCore<T> AndActive<T1>(Expression<Func<T1, bool>> expression = null)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.AndActive, expression));

            return this;
        }

        public QueryCore<T> OrWhere(Expression<Func<T, bool>> expression)
        {
            return OrWhere<T>(expression);
        }

        public QueryCore<T> OrWhere<T1>(Expression<Func<T1, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.OrWhere, expression));

            return this;
        }

        public QueryCore<T> AndWhere(Expression<Func<T, bool>> expression)
        {
            return AndWhere<T>(expression);
        }

        public QueryCore<T> AndWhere<T1>(Expression<Func<T1, bool>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.AndWhere, expression));

            return this;
        }

        public QueryCore<T> OrderBy(Expression<Func<T, object>> expression)
        {
            return OrderBy<T>(expression);
        }

        public QueryCore<T> OrderBy<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.OrderBy, expression));

            return this;
        }

        public QueryCore<T> ThenBy(Expression<Func<T, object>> expression)
        {
            return ThenBy<T>(expression);
        }

        public QueryCore<T> ThenBy<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.ThenBy, expression));

            return this;
        }

        public QueryCore<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            return OrderByDescending<T>(expression);
        }

        public QueryCore<T> OrderByDescending<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.OrderByDescending, expression));

            return this;
        }

        public QueryCore<T> ThenByDescending(Expression<Func<T, object>> expression)
        {
            return ThenByDescending<T>(expression);
        }

        public QueryCore<T> ThenByDescending<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.ThenByDescending, expression));

            return this;
        }

        public QueryCore<T> GroupBy(Expression<Func<T, object>> expression)
        {
            return GroupBy<T>(expression);
        }

        public QueryCore<T> GroupBy<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.GroupBy, expression));

            return this;
        }

        public QueryCore<T> ThenGroupBy(Expression<Func<T, object>> expression)
        {
            return ThenGroupBy<T>(expression);
        }

        public QueryCore<T> ThenGroupBy<T1>(Expression<Func<T1, object>> expression)
        {
            translator.Add(new QuerySegment(typeof(T1), SegmentState.ThenGroupBy, expression));

            return this;
        }

        public override string ToString()
        {
            return this.translator.Translate();
        }
    }
}
