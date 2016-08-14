using System.Collections.Generic;
using System.Linq;
using Provausio.Data.Collections;

namespace Provausio.Data.Ext
{
    public static class WhereExt
    {
        /// <summary>
        /// Applies the specified filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns></returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, 
            ISearchFilter<T> filter,
            SearchMode mode = SearchMode.Loose, 
            bool caseSensitive = false)
        {
            return mode == SearchMode.Loose
                ? source.Where(x => filter.IsLooseMatch(x, caseSensitive))
                : source.Where(x => filter.IsExactMatch(x, caseSensitive));
        }

        /// <summary>
        /// Applies the specified filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source,
            ISearchFilter<T> filter,
            SearchMode mode = SearchMode.Loose,
            bool caseSensitive = false)
        {
            var asQueryable = source.AsQueryable();
            return Where(asQueryable, filter, mode, caseSensitive);
        }
    }

    public enum SearchMode
    {
        /// <summary>
        /// Searches each included property as an exact phrase
        /// </summary>
        Exact,

        /// <summary>
        /// Searches each included property for any matches with any word in the query phrase.
        /// </summary>
        Loose
    }
}
