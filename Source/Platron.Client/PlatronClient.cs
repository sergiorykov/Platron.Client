using System;
using System.Threading.Tasks;
using Platron.Client.Http;
using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class PlatronClient
    {
        public static readonly Uri PlatronUrl = new Uri("https://www.platron.ru");
        private readonly ApiConnection _connection;

        public PlatronClient(string merchantId, string secretKey) : this(new Credentials(merchantId, secretKey))
        {
        }

        public PlatronClient(Credentials credentials) : this(new Connection(PlatronUrl, credentials))
        {
        }

        public PlatronClient(IConnection connection)
        {
            Ensure.ArgumentNotNull(connection, "connection");
            _connection = new ApiConnection(connection);

            ResultUrl = new ResultUrlClient(_connection);
        }

        public Task<HtmlResponse> InitPaymentAsHtmlAsync(InitPaymentRequest request)
        {
            return _connection.SendAsync(
                ApiUrls.InitPayment(InitPaymentResponseType.HtmlForm), request);
        }

        public Task<InitPaymentResponse> InitPaymentAsync(InitPaymentRequest request)
        {
            return _connection.SendAsync<InitPaymentResponse, InitPaymentResponse.Plain>(
                ApiUrls.InitPayment(InitPaymentResponseType.RedirectLink), request);
        }

        public ResultUrlClient ResultUrl { get; private set; }
    }
}