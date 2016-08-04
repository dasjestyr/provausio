namespace Provausio.Rest.Client.Infrastructure
{
    public enum BodyFormat
    {
        /// <summary>
        /// The value was not explicitly set
        /// </summary>
        Unspecified,

        /// <summary>
        /// text/plain
        /// </summary>
        String,

        /// <summary>
        /// application/json
        /// </summary>
        Json,

        /// <summary>
        /// application/xml
        /// </summary>
        Xml,

        /// <summary>
        /// Byte array
        /// </summary>
        Binary
    }
}
