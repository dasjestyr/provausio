using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Provausio.Parsing.Csv
{
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