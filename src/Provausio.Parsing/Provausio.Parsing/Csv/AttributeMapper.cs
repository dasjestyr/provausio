using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Provausio.Parsing.Csv
{
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
}
