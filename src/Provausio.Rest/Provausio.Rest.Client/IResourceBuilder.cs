using System;
using System.Collections.Generic;
using Provausio.Rest.Client.Infrastructure;

namespace Provausio.Rest.Client
{
    public interface IResourceBuilder
    {
        /// <summary>
        /// Sets the scheme.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        IResourceBuilder WithScheme(Scheme scheme);

        /// <summary>
        /// Sets the host authority.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        IResourceBuilder WithHost(string host);

        /// <summary>
        /// Sets the port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid port range</exception>
        IResourceBuilder WithPort(uint port);

        /// <summary>
        /// Adds the resource/path to the host authority.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IResourceBuilder WithPath(string path);

        /// <summary>
        /// Adds object properties to the resource as a query string
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Object is null, nothing to add</exception>
        IResourceBuilder WithQueryParameters(object parameters);

        /// <summary>
        /// Adds the key value pairs to the resource as a query string
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        IResourceBuilder WithQueryParameters(IEnumerable<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// Adds a key value pair to the resource as a key/value segement
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        IResourceBuilder WithSegmentPair(string name, string value);

        /// <summary>
        /// Builds the URI.
        /// </summary>
        /// <returns></returns>
        Uri BuildUri();

        /// <summary>
        /// Sets the attatched client.
        /// </summary>
        /// <param name="client">The client.</param>
        IResourceBuilder WithClient(RestClient client);

        /// <summary>
        /// Returns the attached client.
        /// </summary>
        /// <returns></returns>
        RestClient AsClient();

        string ToString();
    }
}