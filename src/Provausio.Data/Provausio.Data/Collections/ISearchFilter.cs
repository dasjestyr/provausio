namespace Provausio.Data.Collections
{
    public interface ISearchFilter<in T>
    {
        /// <summary>
        /// Returns true if any property in the target object matches any word in the query phrase.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>
        ///   <c>true</c> if [is loose match] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLooseMatch(T target, bool caseSensitive);

        /// <summary>
        /// Returns true if any property in the target object matches the query phrase word for word.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>
        ///   <c>true</c> if [is exact match] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        bool IsExactMatch(T target, bool caseSensitive);
    }
}