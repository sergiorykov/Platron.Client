using Platron.Client.Http;
using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class ErrorApiException : ApiHttpException
    {
        public ErrorApiException(PlatronError error, IHttpResponse httpResponse)
            : base(error.Description)
        {
            Ensure.ArgumentNotNull(error, nameof(error));
            Ensure.ArgumentNotNull(httpResponse, nameof(httpResponse));

            Error = error;
            HttpResponse = httpResponse;
        }

        public PlatronError Error { get; }
    }
}