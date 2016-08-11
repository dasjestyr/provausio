using System;

namespace Provausio.Parsing
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ArrayPropertyAttribute : Attribute
    {

        /// <summary>
        /// Gets or sets the index from which the property will obtain its value in the source array.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayPropertyAttribute"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public ArrayPropertyAttribute(int index)
        {
            Index = index;
        }
    }
}
