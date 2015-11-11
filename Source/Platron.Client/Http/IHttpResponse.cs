using System;
using System.Collections.Generic;
using System.Net;

namespace Platron.Client.Http
{
    /// <summary>
    ///     Represents a generic HTTP response
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        ///     Raw response body.
        /// </summary>
        string Body { get; }

        /// <summary>
        ///     Information about the API.
        /// </summary>
        IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        ///     The response status code.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        ///     The content type of the response.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        ///     Request url.
        /// </summary>
        Uri RequestUri { get; }
    }
}