using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Provausio.Data.Collections
{
    public class PropertyFilter<T> : ISearchFilter<T>
    {
        private readonly string _query;
        private readonly List<Expression<Func<T, object>>> _properties;

        public IReadOnlyList<Expression<Func<T, object>>> IncludedProperties => _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilter{T}" /> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="includeProperties">The included properties.</param>
        public PropertyFilter(string query, params Expression<Func<T, object>>[] includeProperties)
        {
            _query = query;
            _properties = new List<Expression<Func<T, object>>>();

            Include(includeProperties);
        }

        /// <summary>
        /// Includes the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void Include(params Expression<Func<T, object>>[] properties)
        {
            if (!properties.Any())
                return;

            _properties.AddRange(properties);
        }

        /// <summary>
        /// Includes all properties of the object specified as T.
        /// </summary>
        public void IncludeAll()
        {
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanRead))
            {
                var expr = ToExpression(
                    property.Name,
                    typeof(T),
                    property.PropertyType.IsValueType);

                _properties.Add(expr);
            }
        }

        public IQueryable<T> Apply(IQueryable<T> source)
        {
            if (string.IsNullOrEmpty(_query))
                return source;

            var searchWords = _query
                .Split(' ')
                .Select(w => w.ToLower());

            var predicates =
                _properties.Select(
                    selector => selector.Compose(
                        value => value != null &&
                        searchWords.Any(w => value.ToString().ToLower().Contains(w))));

            var filter = predicates.Aggregate(
                PredicateBuilder.False<T>(),
                (aggregate, next) => aggregate.Or(next));

            return source.Where(filter);
        }

        public IEnumerable<T> Apply(IEnumerable<T> source)
        {
            var queryable = source.AsQueryable();
            return Apply(queryable);
        }

        private static Expression<Func<T, object>> ToExpression(string fieldName, Type target, bool isValueType)
        {
            var param = Expression.Parameter(target);
            Expression field = Expression.PropertyOrField(param, fieldName);

            if (isValueType)
                field = Expression.Convert(field, typeof(object));

            return Expression.Lambda<Func<T, object>>(field, param);
        }
    }
}
