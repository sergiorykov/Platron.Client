using Platron.Client.Http;

namespace Platron.Client
{
    /// <summary>
    ///     Represents error in validating service response signature.
    /// </summary>
    public sealed class InvalidResponseApiException : ApiException
    {
        /// <summary>
        ///     Constructs an instance of exception.
        /// </summary>
        public InvalidResponseApiException(string message, IHttpResponse httpResponse) : base(message)
        {
            HttpResponse = httpResponse;
        }
    }
}