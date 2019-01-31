using System.Collections.Generic;
using System.Linq;

namespace Project.Framework
{
    internal sealed class QueryTranslator
    {
        private QueryPack pack;
        private IList<QuerySegment> segments;

        public QueryTranslator()
        {
            this.pack = new QueryPack();
            this.segments = new List<QuerySegment>();
        }

        public void Add(QuerySegment segment)
        {
            this.segments.Add(segment);
        }

        public string Translate()
        {
            this.segments.Each(model => Translate(model));

            return this.pack.ToString();
        }

        private void Translate(QuerySegment segment)
        {
            switch (segment.State)
            {
                case SegmentState.SqlText:
                    TranslateSqlText(segment);
                    return;
                case SegmentState.Top:
                    TranslateTop(segment);
                    return;
                case SegmentState.Count:
                    TranslateCount(segment);
                    return;
                case SegmentState.Select:
                    TranslateSelect(segment);
                    return;
                case SegmentState.Min:
                case SegmentState.Max:
                    TranslateFunction(segment);
                    return;
                case SegmentState.InnerJoin:
                case SegmentState.LeftJoin:
                case SegmentState.RightJoin:
                    TranslateJoin(segment);
                    return;
                case SegmentState.InnerJoinTempTable:
                    TranslateJoinTempTable(segment);
                    return;
                case SegmentState.Where:
                case SegmentState.OrWhere:
                case SegmentState.AndWhere:
                    TranslateWhere(segment);
                    return;
                case SegmentState.WhereActive:
                case SegmentState.AndActive:
                    TranslateWhereActive(segment);
                    return;
                case SegmentState.OrderBy:
                case SegmentState.OrderByDescending:
                case SegmentState.ThenBy:
                case SegmentState.ThenByDescending:
                    TranslateOrderBy(segment);
                    return;
                case SegmentState.GroupBy:
                case SegmentState.ThenGroupBy:
                    TranslateGroupBy(segment);
                    return;
                default:
                    return;
            }
        }

        private void TranslateTop(QuerySegment segment)
        {
            this.pack.State = QueryState.Select;
            this.pack.AppendFormat("SELECT TOP ({0}) ", segment.Count.ToString());

            var table = pack.GetTable(segment.Type);
            if (segment.Expression == null)
            {
                this.pack.AppendFormat("{0}.* ", table.Alias);
            }
            else
            {
                ExpressionTranslator.Translate(segment.Expression.Body, pack);
                pack.Append(string.Join(", ", pack.SelectFields));
                pack.SelectFields.Clear();
            }

            this.pack.AppendFormat(" FROM {0} AS {1}", table.Name, table.Alias);
        }

        private void TranslateCount(QuerySegment segment)
        {
            this.pack.State = QueryState.Select;
            this.pack.Append("SELECT COUNT(");
            if (segment.Expression == null)
            {
                this.pack.Append("1");
            }
            else
            {
                this.pack.Append(segment.Distinct ? "DISTINCT " : "");
                ExpressionTranslator.Translate(segment.Expression.Body, pack);
                pack.Append(string.Join(", ", pack.SelectFields));
                pack.SelectFields.Clear();
            }

            this.pack.Append(")");
            var table = pack.GetTable(segment.Type);
            this.pack.AppendFormat(" FROM {0} AS {1}", table.Name, table.Alias);
        }

        private void TranslateSelect(QuerySegment segment)
        {
            this.pack.State = QueryState.Select;
            this.pack.Append(segment.Distinct ? "SELECT DISTINCT " : "SELECT ");

            var table = pack.GetTable(segment.Type);
            if (segment.Expression == null)
            {
                this.pack.AppendFormat("{0}.* ", table.Alias);
            }
            else
            {
                ExpressionTranslator.Translate(segment.Expression.Body, pack);
                pack.Append(string.Join(", ", pack.SelectFields));
                pack.SelectFields.Clear();
            }

            this.pack.AppendFormat(" FROM {0} AS {1}", table.Name, table.Alias);
        }

        private void TranslateFunction(QuerySegment segment)
        {
            this.pack.State = QueryState.Select;

            this.pack.AppendFormat("SELECT {0}(", segment.StateName);
            ExpressionTranslator.Translate(segment.Expression.Body, pack);
            pack.Append(string.Join(", ", pack.SelectFields));
            pack.SelectFields.Clear();
            this.pack.Append(")");

            var table = pack.GetTable(segment.Type);
            this.pack.AppendFormat(" FROM {0} AS {1}", table.Name, table.Alias);
        }

        private void TranslateJoin(QuerySegment segment)
        {
            this.pack.State = QueryState.Join;

            var table = pack.GetTable(segment.Type);
            this.pack.AppendFormat(" {0} {1} AS {2} ON ", segment.StateName, table.Name, table.Alias);
            ExpressionTranslator.Translate(segment.Expression.Body, pack);
        }

        private void TranslateJoinTempTable(QuerySegment segment)
        {
            this.pack.State = QueryState.Join;

            this.pack.AppendFormat(" {0} {1} AS {2}", segment.StateName, segment.TempTableName, segment.TempTableAlias);
            this.pack.AppendFormat(" ON {0}.Item=", segment.TempTableAlias);
            ExpressionTranslator.Translate(segment.Expression.Body, pack);
        }

        private void TranslateWhere(QuerySegment segment)
        {
            this.pack.State = QueryState.Where;

            this.pack.Append($" {segment.StateName} ");
            ExpressionTranslator.Translate(segment.Expression.Body, pack);
        }

        private void TranslateWhereActive(QuerySegment segment)
        {
            this.pack.State = QueryState.Where;

            var table = pack.GetTable(segment.Type);
            this.pack.AppendFormat(" {0} {1}.IsActive=1 ", segment.StateName, table.Alias);

            if (segment.Expression != null)
            {
                this.pack.Append(" AND ");
                ExpressionTranslator.Translate(segment.Expression.Body, pack);
            }
        }

        private void TranslateOrderBy(QuerySegment segment)
        {
            this.pack.State = QueryState.OrderBy;

            if (segment.State == SegmentState.OrderBy || segment.State == SegmentState.OrderByDescending)
            {
                this.pack.AppendFormat(" {0} ", segment.StateName);
            }
            else
            {
                this.pack.Append(", ");
            }

            ExpressionTranslator.Translate(segment.Expression.Body, pack);
            this.pack.AppendFormat(" {0}", segment.OrderDirection);
        }

        private void TranslateGroupBy(QuerySegment segment)
        {
            this.pack.State = QueryState.GroupBy;

            if (segment.State == SegmentState.GroupBy)
            {
                this.pack.AppendFormat(" {0} ", segment.StateName);
            }
            else
            {
                this.pack.Append(", ");
            }

            ExpressionTranslator.Translate(segment.Expression.Body, pack);
        }

        private void TranslateSqlText(QuerySegment segment)
        {
            if (segment.State == SegmentState.SqlText)
            {
                this.pack.AppendFormat(" {0}", segment.SqlText);
            }
        }
    }
}
