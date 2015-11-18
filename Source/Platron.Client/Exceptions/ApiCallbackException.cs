using System;

namespace Platron.Client
{
    /// <summary>
    ///     Represents errors that occur from the Platron API.
    /// </summary>
    public abstract class ApiCallbackException : ApiException
    {
        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        protected ApiCallbackException()
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected ApiCallbackException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Inner exception.</param>
        protected ApiCallbackException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        ///     Callback uri.
        /// </summary>
        public Uri Uri { get; protected set; }
    }
}