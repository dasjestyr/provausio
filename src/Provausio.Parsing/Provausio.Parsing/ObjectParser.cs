using System;

namespace Provausio.Parsing
{
    public abstract class ObjectParser<T> : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        /// Gets or sets the last parsed line.
        /// </summary>
        /// <value>
        /// The current line.
        /// </value>
        public T CurrentLine { get; protected set; }

        /// <summary>
        /// Reads the next line.
        /// </summary>
        /// <returns></returns>
        public abstract bool ReadNext();

        public void Dispose()
        {
            if (_isDisposed) return;

            Dispose(true);
            _isDisposed = true;

            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}
