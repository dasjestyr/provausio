using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Provausio.Parsing
{
    public class ArrayObjectMapper<T>
        where T : new()
    {
        private readonly Dictionary<int, Expression<Func<T, object>>> _mappers;
        private readonly Dictionary<int, Func<string, string>> _valueCallbacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayObjectMapper{T}"/> class.
        /// </summary>
        public ArrayObjectMapper()
        {
            _mappers = new Dictionary<int, Expression<Func<T, object>>>();
            _valueCallbacks = new Dictionary<int, Func<string, string>>();
        }

        /// <summary>
        /// Defines a mapper for the value located at the specified index.
        /// </summary>
        /// <param name="index">The index of the value that will be mapped.</param>
        /// <param name="mapping">Expression defining the property on the target object to which the value will be mapped.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        public void AddPropertyIndex(int index, Expression<Func<T, object>> mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            _mappers.Add(index, mapping);
        }

        /// <summary>
        /// Defines a mapper for the value located at the specified index.
        /// </summary>
        /// <param name="index">The index of the value that will be mapped.</param>
        /// <param name="mapping">Expression defining the property on the target object to which the value will be mapped.</param>
        /// <param name="valueCallback">A callback that will be run against the value before assigning it to the target object.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        /// <exception cref="ArgumentNullException">valueCallback</exception>
        public void AddPropertyIndex(int index, Expression<Func<T, object>> mapping, Func<string, string> valueCallback)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            if (valueCallback == null)
                throw new ArgumentNullException(nameof(valueCallback));

            _mappers.Add(index, mapping);
            _valueCallbacks.Add(index, valueCallback);
        }

        /// <summary>
        /// Maps the object using the provided string array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public T MapObject(string[] source)
        {
            if (!_mappers.Any())
                throw new InvalidOperationException("No mappers were added!");

            var target = new T();
            var maxIndex = _mappers.Max(m => m.Key);

            for (var i = 0; i < source.Length && i <= maxIndex; i++)
            {
                if (!_mappers.ContainsKey(i))
                    continue;

                var propertyMapper = _mappers[i];

                var memberSelector = propertyMapper.Body as MemberExpression;

                var propertyInfo = memberSelector?.Member as PropertyInfo;
                if (propertyInfo == null)
                    continue;

                var value = GetValue(i, source);

                if (_valueCallbacks.ContainsKey(i))
                {
                    var callback = _valueCallbacks[i];
                    value = callback(value);
                }

                propertyInfo.SetValue(target, value, null);
            }

            return target;
        }

        private static string GetValue(int index, IReadOnlyList<string> source)
        {
            var value = source.Count < index + 1
                ? string.Empty
                : source[index];

            return value;
        }
    }
}
