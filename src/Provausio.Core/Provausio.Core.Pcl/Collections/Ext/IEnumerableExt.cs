using System;
using System.Collections.Generic;
using System.Linq;

namespace Provausio.Core.Pcl.Collections.Ext
{
    public static class EnumerableChunk
    {
        /// <summary>
        /// Chunks the specified size. This method will iterate the source list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));

            var sourceArr = source as T[] ?? source.ToArray();

            while (sourceArr.Any())
            {
                yield return sourceArr.Take(size);
                sourceArr = sourceArr.Skip(size).ToArray();
            }
        }
    }
}
