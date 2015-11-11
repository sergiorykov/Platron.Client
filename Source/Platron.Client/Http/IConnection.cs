using System;
using System.Threading.Tasks;
using Platron.Client.Http.Plain;

namespace Platron.Client.Http
{
    public interface IConnection
    {
        Task<IApiResponse<TPlainResponse>> SendAsync<TPlainResponse>(Uri uri, ClientRequest request)
            where TPlainResponse : PlainResponse, new();

        Task<IApiResponse<string>> SendAsync(Uri uri, ClientRequest request);
    }
}