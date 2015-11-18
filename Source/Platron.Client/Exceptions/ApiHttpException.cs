using System;
using Platron.Client.Http;

namespace Platron.Client
{
    /// <summary>
    ///     Represents errors that occur from the Platron API.
    /// </summary>
    public abstract class ApiHttpException : ApiException
    {
        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        protected ApiHttpException()
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected ApiHttpException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Inner exception.</param>
        protected ApiHttpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Http response.
        /// </summary>
        public IHttpResponse HttpResponse { get; protected set; }
    }
}