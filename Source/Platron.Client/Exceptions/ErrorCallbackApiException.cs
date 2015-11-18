using System;
using Platron.Client.Utils;

namespace Platron.Client
{
    /// <summary>
    ///     Represents error in validating service callback signature.
    /// </summary>
    public sealed class ErrorCallbackApiException : ApiCallbackException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public ErrorCallbackApiException(Uri uri, PlatronError error) : base(error.Description)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(error, nameof(error));

            Uri = uri;
            Error = error;
        }

        public PlatronError Error { get; }
    }
}