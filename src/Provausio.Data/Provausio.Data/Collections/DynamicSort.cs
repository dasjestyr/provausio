using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Provausio.Data.Collections
{
    public class DynamicSort<T>
    {
        private readonly Dictionary<string, Tuple<Expression<Func<T, object>>, bool>> _mappings;

        public DynamicSort()
        {
            _mappings = new Dictionary<string, Tuple<Expression<Func<T, object>>, bool>>();
        }

        /// <summary>
        /// Registers a string against an actual property on the target object.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="ascendingKey">The ascending key.</param>
        /// <param name="descendingKey">The descending key.</param>
        /// <param name="orderByMember">The order by member.</param>
        public void AddKey<TKey>(string ascendingKey, string descendingKey, Expression<Func<T, TKey>> orderByMember)
        {
            // make conversion to account for value types and reference types
            Expression expr = Expression.Convert(orderByMember.Body, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(expr, orderByMember.Parameters);

            _mappings.Add(ascendingKey, new Tuple<Expression<Func<T, object>>, bool>(lambda, false));
            _mappings.Add(descendingKey, new Tuple<Expression<Func<T, object>>, bool>(lambda, true));
        }

        /// <summary>
        /// Applies the sort to the specified query.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IQueryable<T> Apply(string key, IQueryable<T> query)
        {
            if(string.IsNullOrEmpty(key) || !_mappings.ContainsKey(key))
                return query;

            var mapping = _mappings[key];

            var newQuery = mapping.Item2
                ? query.OrderByDescending(mapping.Item1)
                : query.OrderBy(mapping.Item1);

            return newQuery;
        }

        /// <summary>
        /// Applies the sort to the specified query.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IQueryable<T> Apply(string key, IEnumerable<T> query)
        {
            return Apply(key, query.AsQueryable());
        }
    }
}
