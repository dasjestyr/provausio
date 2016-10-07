using System.Net.Http;

namespace Provausio.Rest.Client
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class WebRequest
    {
        /// <summary>
        /// Gets the resource builder.
        /// </summary>
        /// <value>
        /// The resource builder.
        /// </value>
        internal IResourceBuilder ResourceBuilder { get; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public HttpMethod Method { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequest"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        protected WebRequest(HttpMethod method)
            : this(method, new ResourceBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebRequest"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="builder">The builder.</param>
        protected WebRequest(HttpMethod method, IResourceBuilder builder)
        {
            Method = method;
            ResourceBuilder = builder;
        }

        /// <summary>
        /// To be executed when GetHttpRequest() is invoked.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected abstract void SetRequest(IResourceBuilder builder);

        /// <summary>
        /// Gets the HTTP request. 
        /// </summary>
        /// <returns></returns>
        public virtual HttpRequestMessage GetHttpRequest()
        {
            SetRequest(ResourceBuilder);

            var uri = ResourceBuilder.BuildUri();
            var request = new HttpRequestMessage(Method, uri);

            // short circuit
            if (Method != HttpMethod.Post && Method != HttpMethod.Put)
                return request;

            var content = GetContent();
            if (content != null)
                request.Content = GetContent();

            return request;
        }

        /// <summary>
        /// Gets the http content that will be send along with the web request. This will be called when GetHttpRequest() is invoked.
        /// </summary>
        /// <returns></returns>
        public virtual HttpContent GetContent()
        {
            return null;
        }
    }
}
