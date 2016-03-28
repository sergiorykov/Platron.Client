using Nancy;
using Platron.Client.Extensions;
using Platron.Client.Http.Callbacks;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Platron.Client.Tests
{
    public sealed class ReadmeTests
    {
        [Fact(Skip = "Used in readme.md")]
        public async Task GettingStarted_SampleClient_Succeeds()
        {
            var credentials = new Credentials("0000", "asdffsasdfasdfasdf");
            var client = new PlatronClient(credentials);

            // ensure that your server listens on that address and accepts GET request
            var resultUrl = new Uri("https://my.server.com/platron/result");

            var request = new InitPaymentRequest(1.Rub(), "Order payment")
            {
                OrderId = "#1234567890",
                UserPhone = "+79990001112",
                ResultUrl = resultUrl
            };

            InitPaymentResponse response = await client.InitPaymentAsync(request).ConfigureAwait(false);
            await SendToUserAsync(response.RedirectUrl).ConfigureAwait(false);
        }

        private async Task SendToUserAsync(Uri payment)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }

        public sealed class PlatronModule : NancyModule
        {
            private readonly PlatronClient _platronClient;

            public PlatronModule(PlatronClient platronClient)
            {
                // IoC container will make us super-duper happy and gives us a client.
                _platronClient = platronClient;

                Get["/platron/result", true] = async (_, ct) =>
                {
                    CallbackResponse response = await CompleteOrderAsync(Request.Url);
                    return AsXml(response);
                };
            }

            private async Task<CallbackResponse> CompleteOrderAsync(Uri resultUrl)
            {
                ResultUrlRequest request = _platronClient.ResultUrl.Parse(resultUrl);
                CallbackResponse response = _platronClient.ResultUrl.ReturnOk(request, "Order completed");
                return await Task.FromResult(response);
            }

            private Response AsXml(CallbackResponse response)
            {
                return new Response
                {
                    ContentType = "application/xml; charset:utf-8",
                    Contents = stream =>
                    {
                        var data = Encoding.UTF8.GetBytes(response.Content);
                        stream.Write(data, 0, data.Length);
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }
        }
    }
}