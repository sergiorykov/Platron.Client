using System;
using Platron.Client.Http;

namespace Platron.Client
{
    /// <summary>
    ///     Represents errors that occur from the Platron API.
    /// </summary>
    public abstract class ApiException : Exception
    {
        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        public ApiException()
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ApiException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Http response.
        /// </summary>
        public IHttpResponse HttpResponse { get; protected set; }
    }
}