using Platron.Client.Http.Plain;
using Platron.Client.Serializers;

namespace Platron.Client.Http
{
    public interface IXmlPipeline : IXmlSerializer
    {
        IApiResponse<TPlainResponse> Deserialize<TPlainResponse>(IHttpResponse response)
            where TPlainResponse : PlainResponse;

        string Serialize(ApiRequest request);
    }
}