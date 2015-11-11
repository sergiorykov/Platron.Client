using Platron.Client.Http;

namespace Platron.Client
{
    /// <summary>
    ///     Represents error in validating service response signature.
    /// </summary>
    public sealed class InvalidSignatureApiException : ApiException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public InvalidSignatureApiException(IHttpResponse response) : base("invalid response signature")
        {
            HttpResponse = response;
        }
    }
}