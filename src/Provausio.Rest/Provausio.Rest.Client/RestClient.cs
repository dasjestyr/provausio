using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Provausio.Rest.Client.Infrastructure;

namespace Provausio.Rest.Client
{
    public class RestClient : IResourceBuilder
    {
        private readonly IResourceBuilder _builder;

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public HttpMessageHandler Handler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        public RestClient()
            : this(new ResourceBuilder())
        {
            _builder.WithClient(this);
        }

        public RestClient(Scheme scheme, string host) 
            : this()
        {
            _builder.WithClient(this);
            _builder.WithScheme(scheme).WithHost(host);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient"/> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public RestClient(IResourceBuilder builder)
        {
            _builder = builder;
            _builder.WithClient(this);
        }

        /// <summary>
        /// Executes a GET request asychronously
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync()
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _builder.BuildUri());
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Executes a DELETE request asychronously
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteAsync()
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, _builder.BuildUri());
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Executes a POST request asychronously
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync()
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _builder.BuildUri());
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Executes a POST request asychronously. Will also attach a payload with the request.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(HttpContent content)
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _builder.BuildUri()) {Content = content};
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Executes a PUT request asychronously. Will also attach a payload with the request.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync()
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, _builder.BuildUri());
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Executes a PUT request asychronously. Will also attach a payload with the request.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PutAsync(HttpContent content)
        {
            using (var client = GetClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, _builder.BuildUri()) {Content = content};
                return await client.SendAsync(request);
            }
        }

        private HttpClient GetClient()
        {
            return Handler == null
                ? new HttpClient()
                : new HttpClient(Handler);
        }

        #region -- IResourceBuilder Implementation --

        public IResourceBuilder WithScheme(Scheme scheme)
        {
            return _builder.WithScheme(scheme);
        }

        public IResourceBuilder WithHost(string host)
        {
            return _builder.WithHost(host);
        }

        public IResourceBuilder WithPort(uint port)
        {
            return _builder.WithPort(port);
        }

        public IResourceBuilder WithPath(string path)
        {
            return _builder.WithPath(path);
        }

        public IResourceBuilder WithQueryParameters(object parameters)
        {
            return _builder.WithQueryParameters(parameters);
        }

        public IResourceBuilder WithQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return _builder.WithQueryParameters(parameters);
        }

        public IResourceBuilder WithSegmentPair(string name, string value)
        {
            return _builder.WithSegmentPair(name, value);
        }

        public Uri BuildUri()
        {
            return _builder.BuildUri();
        }

        public IResourceBuilder WithClient(RestClient client)
        {
            _builder.WithClient(client);
            return this;
        }

        public RestClient AsClient()
        {
            return _builder.AsClient();
        }

        #endregion
    }
}
