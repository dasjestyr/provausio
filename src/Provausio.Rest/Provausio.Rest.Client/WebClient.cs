using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Provausio.Rest.Client
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class WebClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClient"/> class.
        /// </summary>
        public WebClient()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebClient"/> class.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public WebClient(HttpMessageHandler handler)
        {
            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// Executes the web request and returns a raw <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(WebRequest request)
        {
            var httpRequest = request.GetHttpRequest();
            
            var response = await _httpClient
                .SendAsync(httpRequest)
                .ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Executes the web request and returns a raw <see cref="HttpResponseMessage"/>
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await _httpClient
                .SendAsync(request)
                .ConfigureAwait(false);

            return response;
        }

        /// <summary>
        /// Executes the request and attempts to deserialize the response body. If the body is null or empty, a null will be returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(WebRequest request)
            where T : class, new()
        {
            var response = await SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var obj = await DeserializeBody<T>(response);

            return obj;
        }

        /// <summary>
        /// Executes the request and runs the provider mapper delgate, using the response body as an input parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <param name="mapper">The mapper.</param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(WebRequest request, Func<string, T> mapper)
            where T : class
        {
            var response = await SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            return mapper(body);
        }

        private static async Task<T> DeserializeBody<T>(HttpResponseMessage response)
            where T : class, new()
        {
            // TODO: support other content types.

            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(body))
                return null;

            return await Task.Run(() => JsonConvert.DeserializeObject<T>(body)).ConfigureAwait(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _httpClient.Dispose();
        }
    }
}
