using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Provausio.Parsing
{
    public class DelimitedStringParser<T> : ObjectParser<T>
        where T : class, new()
    {
        private readonly StreamReader _streamReader;
        private readonly StringArrayObjectMapper<T> _mapper;
        private readonly string[] _delimiters;

        /// <summary>
        /// Gets the base stream.
        /// </summary>
        /// <value>
        /// The base stream.
        /// </value>
        public Stream BaseStream => _streamReader.BaseStream;

        /// <summary>
        /// Specifies whether or not the first row in the source file contains column names.
        /// </summary>
        public bool FirstRowHeaders { get; set; }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        public ExplicitMapper<T> Mapper => _mapper.IndexMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedStringParser{T}" /> class.
        /// </summary>
        /// <param name="source">The source string</param>
        /// <param name="delimiters">The delimiters.</param>
        /// <exception cref="ArgumentException">Unknown or unspecified source type</exception>
        public DelimitedStringParser(string source, params string[] delimiters)
            : this((Stream) null, delimiters)
        {
            if(string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            var bytes = Encoding.UTF8.GetBytes(source);
            var ms = new MemoryStream(bytes);

            _streamReader = new StreamReader(ms);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedStringParser{T}"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="delimiters">The delimiters.</param>
        public DelimitedStringParser(Stream stream, params string[] delimiters)
        {
            if(stream == null)
                throw new ArgumentNullException(nameof(stream));

            if(!stream.CanRead)
                throw new InvalidOperationException("Cannot initialize a parser with a stream that is already at the end.");
            
            _delimiters = delimiters;
            _mapper = new StringArrayObjectMapper<T>();
            _streamReader = new StreamReader(stream);
        }

        /// <summary>
        /// Reads all lines in a single run.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReadAll()
        {
            var result = await Task.Run(() =>
            {
                var items = new List<T>();
                while (ReadNext())
                {
                    items.Add(CurrentLine);
                }
                return items;
            });

            return result;
        }

        /// <summary>
        /// Reads the next line. The result will be placed in the CurrentLine property. Returns false if there are no more values that can be read.
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            string[] headers = null;
            if (FirstRowHeaders)
                headers = _streamReader.ReadLine().Split(_delimiters, StringSplitOptions.None);

            string line = null;
            CurrentLine = null;

            while (string.IsNullOrEmpty(line) || (CurrentLine == null && !_streamReader.EndOfStream))
            {
                line = _streamReader.ReadLine();
                CurrentLine = MapLine(line, headers);
            }
            
            return CurrentLine != null;
        }

        /// <summary>
        /// Reads the delimited string and attempts to map it using the previously set mapped.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        protected virtual T MapLine(string line, string[] headers)
        {
            var parts = line.Split(_delimiters, StringSplitOptions.None);
            var obj = _mapper.MapObject(parts, headers);

            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _streamReader.Dispose();
        }
    }
}
