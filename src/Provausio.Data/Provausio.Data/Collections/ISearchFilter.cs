using System.Linq;

namespace Provausio.Data.Collections
{
    public interface ISearchFilter<T>
    {
        /// <summary>
        /// Applies the filter to the specified query and returns the result.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        IQueryable<T> Apply(IQueryable<T> source);
    }
}