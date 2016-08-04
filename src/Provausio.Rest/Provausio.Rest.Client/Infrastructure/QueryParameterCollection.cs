using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Provausio.Rest.Client.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{String, String}" />
    internal class QueryParameterCollection : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        public QueryParameterCollection()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public QueryParameterCollection(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var element in parameters)
            {
                Add(element.Key, element.Value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameterCollection"/> class.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public QueryParameterCollection(object parameters)
        {
            var hasProperties = parameters
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Any(p => p.CanRead);

            if(!hasProperties)
                throw new ArgumentException("No properties were found on the provided object");

            Add(parameters);
        }

        /// <summary>
        /// Adds the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Add(object parameters)
        {
            var properties = parameters
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(p => p.CanRead);

            foreach (var property in properties)
            {
                var value = property.GetValue(parameters).ToString();
                Add(property.Name, value);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!this.Any())
                return string.Empty;

            var values = this.Select(v => $"{WebUtility.UrlEncode(v.Key)}={WebUtility.UrlEncode(v.Value)}");
            return string.Join("&", values);
        }
    }
}
