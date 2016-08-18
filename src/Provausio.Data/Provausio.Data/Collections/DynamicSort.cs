using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Provausio.Data.Collections
{
    public class DynamicSort<T>
    {
        private const string DefaultSortKey = "@@default@@";
        private readonly Dictionary<string, Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>>> _selectors;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSort{T}" /> class.
        /// </summary>
        /// <param name="defaultSortItem">The default sort item.</param>
        /// <exception cref="System.ArgumentNullException">defaultSortItem</exception>
        public DynamicSort(Expression<Func<T, object>> defaultSortItem)
        {
            if (defaultSortItem == null)
                throw new ArgumentNullException(nameof(defaultSortItem));

            _selectors = new Dictionary<string, Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>>>
            {
                {DefaultSortKey, new Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>>(defaultSortItem, false, new List<LambdaExpression>())}
            };
        }

        /// <summary>
        /// Registers a string against an actual property on the target object.
        /// </summary>
        /// <param name="ascendingKey">The ascending key.</param>
        /// <param name="descendingKey">The descending key.</param>
        /// <param name="orderByMember">The order by member.</param>
        /// <param name="thenBy">The then by.</param>
        public void AddKey(string ascendingKey, string descendingKey, Expression<Func<T, dynamic>> orderByMember, params Expression<Func<T, dynamic>>[] thenBy)
        {
            // make conversion to account for value types and reference types
            _selectors.Add(
                ascendingKey.ToLower(),
                new Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>>(orderByMember, false, thenBy));

            _selectors.Add(
                descendingKey.ToLower(),
                new Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>>(orderByMember, true, thenBy));
        }

        /// <summary>
        /// Applies the sort to the specified query.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IOrderedQueryable<T> Apply(string key, IQueryable<T> query)
        {
            key = key.ToLower();
            bool isDescending;

            var selectorGroup = GetSelector(key, out isDescending);
            dynamic selector = selectorGroup.Item1;

            IOrderedQueryable<T> newQuery = isDescending
                ? Queryable.OrderByDescending(query, selector)
                : Queryable.OrderBy(query, selector);

            foreach (dynamic thenby in selectorGroup.Item3)
            {
                newQuery = isDescending
                    ? Queryable.ThenByDescending(newQuery, thenby)
                    : Queryable.ThenBy(newQuery, thenby);
            }

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

        private Tuple<LambdaExpression, bool, IEnumerable<LambdaExpression>> GetSelector(string key, out bool isDescending)
        {
            if (string.IsNullOrEmpty(key) || !_selectors.ContainsKey(key))
            {
                isDescending = false;
                return _selectors[DefaultSortKey];
            }

            var selector = _selectors[key];
            isDescending = selector.Item2;

            return _selectors[key];
        }
    }
}
