using System;

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
        protected ApiException()
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected ApiException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Constructs an instance of ApiException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Inner exception.</param>
        protected ApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}