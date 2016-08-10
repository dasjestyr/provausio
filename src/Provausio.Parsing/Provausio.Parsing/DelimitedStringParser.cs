using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Provausio.Parsing
{
    public abstract class DelimitedStringParser<T> : ObjectParser<T>
        where T : class, new()
    {
        private bool _mapperConfigured;
        private readonly string[] _delimiters;

        protected StreamReader FileStream;
        protected StringArrayObjectMapper<T> Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedStringParser{T}" /> class.
        /// </summary>
        /// <param name="source">The file path.</param>
        /// <param name="delimiters">The delimiters.</param>
        /// <exception cref="ArgumentException">Unknown or unspecified source type</exception>
        protected DelimitedStringParser(string source, params string[] delimiters)
            : this((StreamReader) null, delimiters)
        {
            if(string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            var bytes = Encoding.UTF8.GetBytes(source);
            var ms = new MemoryStream(bytes);

            FileStream = new StreamReader(ms);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelimitedStringParser{T}"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="delimiters">The delimiters.</param>
        protected DelimitedStringParser(StreamReader stream, params string[] delimiters)
        {
            if(stream == null)
                throw new ArgumentNullException(nameof(stream));

            if(stream.EndOfStream || !stream.BaseStream.CanRead)
                throw new InvalidOperationException("Cannot initialize a parser with a stream that is already at the end.");

            _delimiters = delimiters;
            FileStream = stream;
        }

        /// <summary>
        /// Reads all and then disposes the stream.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReadAll()
        {
            var result = await Task.Run(() =>
            {
                using (this)
                {
                    var items = new List<T>();
                    while (ReadNext())
                    {
                        items.Add(CurrentLine);
                    }
                    return items;
                }
            });

            return result;
        }

        /// <summary>
        /// Reads the next line. The result will be placed in the CurrentLine property. Returns false if there are no more values that can be read.
        /// </summary>
        /// <returns></returns>
        public override bool ReadNext()
        {
            if (!_mapperConfigured)
            {
                Mapper = new StringArrayObjectMapper<T>();
                ConfigureMapper();
                _mapperConfigured = true;
            }

            string line = null;
            while (string.IsNullOrEmpty(line) && !FileStream.EndOfStream)
            {
                line = FileStream.ReadLine();
            }

            CurrentLine = null;
            while (CurrentLine == null && !FileStream.EndOfStream)
            {
                CurrentLine = MapLine(line);
            }

            return CurrentLine != null;
        }

        protected abstract void ConfigureMapper();

        /// <summary>
        /// Reads the delimited string and attempts to map it using the previously set mapped.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        protected virtual T MapLine(string line)
        {
            var parts = line.Split(_delimiters, StringSplitOptions.None);
            var obj = Mapper.MapObject(parts);

            return obj;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            FileStream.Dispose();
        }
    }
}
