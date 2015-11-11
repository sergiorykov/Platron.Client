using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    /// <summary>
    ///     Represents a generic HTTP response
    /// </summary>
    internal sealed class HttpResponse : IHttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode, string body, Uri requestUri,
            string contentType = "application/xml", IDictionary<string, string> headers = null)
        {
            Ensure.ArgumentNotNull(body, "body");
            Ensure.ArgumentNotNull(requestUri, "requestUri");
            Ensure.ArgumentNotNull(headers, "headers");

            StatusCode = statusCode;
            Body = body;
            RequestUri = requestUri;
            Headers = new ReadOnlyDictionary<string, string>(headers ?? new Dictionary<string, string>());
            ContentType = contentType;
        }

        /// <summary>
        ///     Request url.
        /// </summary>
        public Uri RequestUri { get; }

        /// <summary>
        ///     Raw response body.
        /// </summary>
        public string Body { get; }

        /// <summary>
        ///     Information about the API.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers { get; }

        /// <summary>
        ///     The response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        ///     The content type of the response.
        /// </summary>
        public string ContentType { get; }

        /// <summary>
        ///     Returns true if content is html.
        /// </summary>
        /// <returns>True, if content type is text/html. False, otherwise.</returns>
        public bool IsHtml()
        {
            return ContentType == "text/html";
        }

        /// <summary>
        ///     Returns true if content is xml.
        /// </summary>
        /// <returns>True, if content type is text/xml. False, otherwise.</returns>
        public bool IsXml()
        {
            return ContentType == "text/xml";
        }
    }
}