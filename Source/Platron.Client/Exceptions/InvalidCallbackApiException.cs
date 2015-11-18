using System;
using Platron.Client.Utils;

namespace Platron.Client
{
    /// <summary>
    ///     Represents error in validating service callback signature.
    /// </summary>
    public sealed class InvalidCallbackApiException : ApiCallbackException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public InvalidCallbackApiException(string message, Uri uri) : base(message)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            Uri = uri;
        }
    }
}