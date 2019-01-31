using System;
using System.Linq.Expressions;

namespace Project.Framework
{
    internal sealed class QuerySegment
    {
        private static string GetStateName(SegmentState state)
        {
            switch (state)
            {
                case SegmentState.Max:
                    return "MAX";
                case SegmentState.Min:
                    return "Min";
                case SegmentState.InnerJoin:
                case SegmentState.InnerJoinTempTable:
                    return "INNER JOIN";
                case SegmentState.LeftJoin:
                    return "LEFT JOIN";
                case SegmentState.RightJoin:
                    return "RIGHT JOIN";
                case SegmentState.Where:
                case SegmentState.WhereActive:
                    return "WHERE";
                case SegmentState.OrWhere:
                    return "OR";
                case SegmentState.AndWhere:
                case SegmentState.AndActive:
                    return "AND";
                case SegmentState.OrderBy:
                case SegmentState.OrderByDescending:
                    return "ORDER BY";
                case SegmentState.GroupBy:
                    return "GROUP BY";
                default:
                    return string.Empty;
            }
        }

        private string tempTableName;

        public int Count { get; set; }

        public bool Distinct { get; set; }

        public Type Type { get; set; }

        public SegmentState State { get; set; }

        public string StateName
        {
            get
            {
                return GetStateName(this.State);
            }
        }

        public string LikeText { get; set; }

        public string SqlText { get; set; }

        public string TempTableName
        {
            get
            {
                return this.tempTableName.StartsWith("#") ? this.tempTableName : $"#{this.tempTableName}";
            }
            set
            {
                this.tempTableName = value;
            }
        }

        public string TempTableAlias
        {
            get
            {
                return !tempTableName.StartsWith("#") ? this.tempTableName : this.tempTableName.TrimStart('#');
            }
        }

        public string OrderDirection
        {
            get
            {
                if (State == SegmentState.OrderBy || State == SegmentState.ThenBy)
                {
                    return "ASC";
                }
                else if (State == SegmentState.OrderByDescending || State == SegmentState.ThenByDescending)
                {
                    return "DESC";
                }

                throw new ArgumentException("Wrong Segment State.");
            }
        }

        public LambdaExpression Expression { get; set; }

        public QuerySegment() { }

        public QuerySegment(Type type, SegmentState state, LambdaExpression expression)
        {
            this.Type = type;
            this.State = state;
            this.Expression = expression;
        }

        public static QuerySegment GetCount(int count, Type type, SegmentState state, LambdaExpression expression)
        {
            return new QuerySegment()
            {
                Count = count,
                Type = type,
                State = state,
                Expression = expression
            };
        }

        public static QuerySegment GetDistinct(bool distinct, Type type, SegmentState state, LambdaExpression expression)
        {
            return new QuerySegment()
            {
                Distinct = distinct,
                Type = type,
                State = state,
                Expression = expression
            };
        }

        public static QuerySegment GetLike(string likeText, Type type, SegmentState state, LambdaExpression expression)
        {
            return new QuerySegment()
            {
                LikeText = likeText,
                Type = type,
                State = state,
                Expression = expression
            };
        }

        public static QuerySegment GetSqlText(string sqlText)
        {
            return new QuerySegment()
            {
                State = SegmentState.SqlText,
                SqlText = sqlText
            };
        }

        public static QuerySegment GetTempTable(string tableName, Type type, SegmentState state, LambdaExpression expression)
        {
            return new QuerySegment
            {
                TempTableName = tableName,
                Type = type,
                State = state,
                Expression = expression
            };
        }
    }

    public enum SegmentState
    {
        SqlText,
        Select,
        Top,
        Max,
        Min,
        Count,
        InnerJoin,
        InnerJoinTempTable,
        LeftJoin,
        RightJoin,
        Where,
        WhereActive,
        AndActive,
        OrWhere,
        AndWhere,
        OrderBy,
        ThenBy,
        OrderByDescending,
        ThenByDescending,
        GroupBy,
        ThenGroupBy
    }
}
