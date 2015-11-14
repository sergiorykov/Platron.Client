using Platron.Client.Http;
using Platron.Client.Http.Plain;

namespace Platron.Client
{
    public sealed class ErrorApiException : ApiException
    {
        public ErrorApiException(PlainErrorResponse error, IHttpResponse httpResponse)
            : base(error.ErrorDescription)
        {
            Error = new PlatronError(error.ErrorCode, error.ErrorDescription);
            HttpResponse = httpResponse;
        }

        public PlatronError Error { get; }
    }
}