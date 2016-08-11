using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Provausio.Parsing
{
    public interface IStringArrayMapper<T>
    {
        T Map(IReadOnlyList<string> source, T target);
    }

    public class AttributeMapper<T> : IStringArrayMapper<T>
    {
        public T Map(IReadOnlyList<string> source, T target)
        {
            var properties = GetDecoratedProperties(target);
            if (properties == null)
                return target;

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<ArrayPropertyAttribute>();
                var index = attribute.Index;

                var stringValue = GetValue(index, source);
                var propertyValue = SafeConvert(stringValue, property.PropertyType);
                property.SetValue(target, propertyValue, null);
            }

            return target;
        }

        private static List<PropertyInfo> GetDecoratedProperties(T target)
        {
            var properties = target
                .GetType()
                .GetTypeInfo()
                .DeclaredProperties
                .Where(prop => prop.GetCustomAttributes<ArrayPropertyAttribute>().Any())
                .ToList();

            return properties;
        }

        private static string GetValue(int index, IReadOnlyList<string> source)
        {
            var value = source.Count < index + 1
                ? string.Empty
                : source[index];

            return value;
        }

        private static object SafeConvert(string value, Type destinationType)
        {
            var result = GetDefaultValue(destinationType);

            try
            {
                result = Convert.ChangeType(value, destinationType);
                return result;
            }
            catch (FormatException) { }

            return result;
        }

        private static object GetDefaultValue(Type t)
        {
            return t.GetTypeInfo().IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }

    public class ExplicitMapper<T> : IStringArrayMapper<T>
    {
        private readonly Dictionary<int, Expression<Func<T, object>>> _mappers;
        private readonly Dictionary<int, Func<string, object>> _valueCallbacks;

        public int Count => _mappers.Count;

        public ExplicitMapper()
        {
            _mappers = new Dictionary<int, Expression<Func<T, object>>>();
            _valueCallbacks = new Dictionary<int, Func<string, object>>();
        }

        public T Map(IReadOnlyList<string> source, T target)
        {
            var maxIndex = _mappers.Max(m => m.Key);
            for (var i = 0; i < source.Count && i <= maxIndex; i++)
            {
                if (!_mappers.ContainsKey(i))
                    continue;

                var propertyMapper = _mappers[i];

                // account for value types (unary)
                var unary = propertyMapper.Body as UnaryExpression;
                var memberSelector = unary == null
                    ? propertyMapper.Body as MemberExpression
                    : unary.Operand as MemberExpression;

                var propertyInfo = memberSelector?.Member as PropertyInfo;
                if (propertyInfo == null)
                    continue;

                var value = GetValue(i, source);

                if (_valueCallbacks.ContainsKey(i))
                {
                    // the client provided a callback so don't mess with the type
                    var callback = _valueCallbacks[i];
                    propertyInfo.SetValue(target, callback(value), null);
                }
                else
                {
                    var convertedValue = SafeConvert(value, propertyInfo.PropertyType);
                    propertyInfo.SetValue(target, convertedValue, null);
                }
            }

            return target;
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
        /// <param name="valueCallback">A callback that will be run against the value before assigning it to the target object. Use this for transforming the result value fetched from the source file.</param>
        /// <exception cref="ArgumentNullException">mapping</exception>
        /// <exception cref="ArgumentNullException">valueCallback</exception>
        public void AddPropertyIndex(int index, Expression<Func<T, object>> mapping, Func<string, object> valueCallback)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));

            if (valueCallback == null)
                throw new ArgumentNullException(nameof(valueCallback));

            _mappers.Add(index, mapping);
            _valueCallbacks.Add(index, valueCallback);
        }

        private static string GetValue(int index, IReadOnlyList<string> source)
        {
            var value = source.Count < index + 1
                ? string.Empty
                : source[index];

            return value;
        }

        private static object SafeConvert(string value, Type destinationType)
        {
            var result = GetDefaultValue(destinationType);

            try
            {
                result = Convert.ChangeType(value, destinationType);
                return result;
            }
            catch (FormatException) { }

            return result;
        }

        private static object GetDefaultValue(Type t)
        {
            return t.GetTypeInfo().IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }

    public class HeaderMapper<T> : IStringArrayMapper<T>
    {
        private readonly IReadOnlyList<string> _headers;

        public HeaderMapper(IReadOnlyList<string> headers)
        {
            _headers = headers;
        }

        public T Map(IReadOnlyList<string> source, T target)
        {
            for (var i = 0; i < source.Count; i++)
            {
                if (i > _headers.Count - 1)
                    continue;

                var columnName = _headers[i];
                var property = target
                    .GetType()
                    .GetTypeInfo()
                    .DeclaredProperties
                    .SingleOrDefault(prop => prop.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (property == null)
                    continue;

                var value = SafeConvert(source[i], property.PropertyType);
                property.SetValue(target, value, null);
            }

            return target;
        }

        private static object SafeConvert(string value, Type destinationType)
        {
            var result = GetDefaultValue(destinationType);

            try
            {
                result = Convert.ChangeType(value, destinationType);
                return result;
            }
            catch (FormatException) { }

            return result;
        }

        private static object GetDefaultValue(Type t)
        {
            return t.GetTypeInfo().IsValueType
                ? Activator.CreateInstance(t)
                : null;
        }
    }
}
