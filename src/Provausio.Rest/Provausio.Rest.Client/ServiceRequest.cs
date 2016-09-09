namespace Provausio.Rest.Client
{
    public abstract class ServiceRequest
    {
        /// <summary>
        /// Gets the resource builder.
        /// </summary>
        /// <value>
        /// The resource builder.
        /// </value>
        public abstract IResourceBuilder ResourceBuilder { get; }
    }
}
