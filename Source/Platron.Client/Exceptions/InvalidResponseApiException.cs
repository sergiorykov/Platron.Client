using System;
using Platron.Client.Http;

namespace Platron.Client
{
    /// <summary>
    ///     Represents error in validating service response signature.
    /// </summary>
    public sealed class InvalidResponseApiException : ApiHttpException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public InvalidResponseApiException(string message, IHttpResponse httpResponse) : base(message)
        {
            HttpResponse = httpResponse;
        }

        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public InvalidResponseApiException(string message, IHttpResponse httpResponse, Exception innerException) : base(message, innerException)
        {
            HttpResponse = httpResponse;
        }
    }
}