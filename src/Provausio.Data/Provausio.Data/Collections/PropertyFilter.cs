using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Provausio.Data.Collections
{
    public class PropertyFilter<T> : ISearchFilter<T>
    {
        private readonly string _query;
        private readonly List<Expression<Func<T, object>>> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyFilter{T}" /> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="includedProperties">The included properties.</param>
        public PropertyFilter(string query, params Expression<Func<T, object>>[] includedProperties)
        {
            _query = query;
            _properties = new List<Expression<Func<T, object>>>();

            Include(includedProperties);
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
            foreach (var property in typeof(T).GetTypeInfo().DeclaredProperties.Where(p => p.CanRead))
            {
                var expr = ToExpression(property.Name, typeof(T), property.PropertyType.IsValueType);
                _properties.Add(expr);
            }
        }
        
        /// <summary>
        /// Returns true if any individual words in the search phrase match any words in the source object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>
        ///   <c>true</c> if [is loose match] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLooseMatch(T target, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(_query))
                return true;

            var objectWords = new List<string>();

            foreach (var property in _properties)
            {
                var stringProp = property.Compile()(target).ToString();
                var propertyWords = GetWords(stringProp);
                objectWords.AddRange(propertyWords);
            }

            var queryWords = GetWords(_query);
            return MatchesAny(objectWords, queryWords, caseSensitive);
        }

        /// <summary>
        /// returns true if any property on the object matches the search string exactly.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
        /// <returns>
        ///   <c>true</c> if [is exact match] [the specified target]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExactMatch(T target, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(_query))
                return true;

            return _properties
                .Select(property => property.Compile()(target).ToString())
                .Any(stringProp => _query.Equals(stringProp, caseSensitive 
                    ? StringComparison.Ordinal 
                    : StringComparison.OrdinalIgnoreCase));
        }

        private static string[] GetWords(string source)
        {
            return string.IsNullOrEmpty(source) 
                ? new string[0] 
                : source.Split(' ');
        }

        private static bool MatchesAny(IEnumerable<string> source, string[] compareTo, bool caseSensitive)
        {
            return
                (from sWord in source
                    from cWord in compareTo
                    where sWord.Equals(cWord, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)
                    select sWord).Any();
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
