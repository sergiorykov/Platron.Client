using Platron.Client.Http;
using Platron.Client.Http.Plain;

namespace Platron.Client
{
    public sealed class InvalidResponseApiException : ApiException
    {
        public InvalidResponseApiException(string message, IHttpResponse httpResponse) : base(message)
        {
            HttpResponse = httpResponse;
        }

        public InvalidResponseApiException(PlainErrorResponse response, IHttpResponse httpResponse)
            : base(response.ErrorDescription)
        {
            Response = response;
            HttpResponse = httpResponse;
        }

        public PlainErrorResponse Response { get; }
    }
}