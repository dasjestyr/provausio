namespace Provausio.Parsing.Csv
{
    /// <summary>
    /// Parses a character-delimited file into an object using a mapping definition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringArrayObjectMapper<T>
        where T : new()
    {
        private readonly IStringArrayMapper<T> _attributeMapper;
        private IStringArrayMapper<T> _headerMapper;

        /// <summary>
        /// Gets the index mapper.
        /// </summary>
        /// <value>
        /// The index mapper.
        /// </value>
        public ExplicitMapper<T> IndexMapper { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArrayObjectMapper{T}"/> class.
        /// </summary>
        public StringArrayObjectMapper()
        {
            IndexMapper = new ExplicitMapper<T>();

            _attributeMapper = new AttributeMapper<T>();
        }

        /// <summary>
        /// Maps the object using the provided string array.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="headers">An array of column names.</param>
        /// <returns></returns>
        public T MapObject(string[] source, string[] headers = null)
        {
            var target = new T();

            // map using a priority system. Last dude wins.
            
            // by headers
            if (headers != null)
            {
                if(_headerMapper == null)
                    _headerMapper = new HeaderMapper<T>(headers);

                target = _headerMapper.Map(source, target);
            }

            // then by explicit mapper
            if (IndexMapper != null && IndexMapper.Count > 0)
                target = IndexMapper.Map(source, target);

            // then by attribute
            target = _attributeMapper.Map(source, target);

            return target;
        }
    }
}
