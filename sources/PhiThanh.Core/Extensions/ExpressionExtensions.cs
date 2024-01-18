using System.Linq.Expressions;

namespace PhiThanh.Core.Extensions
{
    public static class ExpressionExtensions
    {
        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            if (first == null && second == null)
                return null!;
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterHelper.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> target,
            params Expression<Func<T, bool>>[] expressions)
        {
            Expression<Func<T, bool>> resultExpression = target;
            if (target != null)
            {
                resultExpression = expressions.Aggregate(resultExpression, (current, t) => current.Compose(t, Expression.AndAlso));
            }
            return resultExpression;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> target,
            params Expression<Func<T, bool>>[] expressions)
        {
            Expression<Func<T, bool>> resultExpression = target;
            if (target != null)
            {
                resultExpression = expressions.Aggregate(resultExpression, (current, t) => current.Compose(t, Expression.OrElse));
            }
            return resultExpression;
        }

        public class ParameterHelper(Dictionary<ParameterExpression, ParameterExpression> map) : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map = map ?? [];

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterHelper(map).Visit(exp);
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression replacement;
                if (map.TryGetValue(node, out replacement!))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }
        }
    }
}
