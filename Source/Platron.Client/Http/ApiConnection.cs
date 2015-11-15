using System;
using System.Threading.Tasks;
using Platron.Client.Http.Callbacks;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public sealed class ApiConnection
    {
        public ApiConnection(IConnection connection)
        {
            Connection = connection;
        }

        public IConnection Connection { get; }
        public ICallbackResponder Callback => Connection.Callback;

        public async Task<TResponse> SendAsync<TResponse, TPlainResponse>(Uri uri, ClientRequest request)
            where TResponse : ClientResponse<TPlainResponse>, new()
            where TPlainResponse : PlainResponse, new()
        {
            Ensure.ArgumentNotNull(uri, "uri");
            Ensure.ArgumentNotNull(request, "request");

            var apiResponse = await Connection.SendAsync<TPlainResponse>(uri, request);

            var response = new TResponse();
            response.Init(apiResponse.Body);

            return response;
        }

        public async Task<HtmlResponse> SendAsync(Uri uri, ClientRequest request)
        {
            Ensure.ArgumentNotNull(uri, "uri");
            Ensure.ArgumentNotNull(request, "request");

            var apiResponse = await Connection.SendAsync(uri, request);
            return new HtmlResponse(apiResponse.Body, apiResponse.HttpResponse.RequestUri);
        }
    }
}