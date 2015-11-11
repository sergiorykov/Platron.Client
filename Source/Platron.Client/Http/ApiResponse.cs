using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public sealed class ApiResponse<TPlainResponse> : IApiResponse<TPlainResponse>
    {
        public ApiResponse(IHttpResponse response, TPlainResponse plainResponse)
        {
            Ensure.ArgumentNotNull(response, "response");
            Ensure.ArgumentNotNull(plainResponse, "plainResponse");

            HttpResponse = response;
            Body = plainResponse;
        }

        public TPlainResponse Body { get; }
        public IHttpResponse HttpResponse { get; }
    }
}