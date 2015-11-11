using System;

namespace Platron.Client
{
    /// <summary>
    ///     Represents connection error due to network related problems.
    /// </summary>
    [Serializable]
    public sealed class ServiceNotAvailableApiException : ApiException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public ServiceNotAvailableApiException()
        {
        }

        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ServiceNotAvailableApiException(string message) : base(message)
        {
        }
    }
}