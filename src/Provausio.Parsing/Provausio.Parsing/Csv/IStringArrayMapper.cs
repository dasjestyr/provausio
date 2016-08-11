using System.Collections.Generic;

namespace Provausio.Parsing.Csv
{
    public interface IStringArrayMapper<T>
    {
        T Map(IReadOnlyList<string> source, T target);
    }
}