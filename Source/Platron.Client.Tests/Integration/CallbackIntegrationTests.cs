using System;
using System.Threading.Tasks;
using Platron.Client.Extensions;
using Platron.Client.Http;
using Platron.Client.TestKit.Emulators;
using Platron.Client.TestKit.Emulators.Nancy;
using Platron.Client.TestKit.Site;
using Xunit;
using Xunit.Abstractions;

namespace Platron.Client.Tests.Integration
{
    public sealed class CallbackIntegrationTests : IClassFixture<CallbackServerEmulator>
    {
        private readonly CallbackServerEmulator _server;
        private readonly ITestOutputHelper _output;

        public CallbackIntegrationTests(CallbackServerEmulator server, ITestOutputHelper output)
        {
            server.Start();

            _server = server;
            _output = output;
        }

        [Trait("Category", Categories.Integration)]
        [Trait("Category", Categories.Manual)]
//        [Fact]
        [Fact(Skip = "Manual execution only. Requires real payment with subsequent rejection.")]
        public async Task FullPayment_ManualPaymentThruBrowser_Succeeds()
        {
            var connection = new Connection(PlatronClient.PlatronUrl, SettingsStorage.Credentials,
                HttpRequestEncodingType.PostWithQueryString);

            var client = new PlatronClient(connection);

            var initPaymentRequest = new InitPaymentRequest(1.01.Rur(), "verifying resulturl")
                                     {
                                         ResultUrl = _server.ResultUrl,
                                         UserPhone = SettingsStorage.PhoneNumber,
                                         OrderId = Guid.NewGuid().ToString("N"),
                                         NeedUserPhoneNotification = true
                                     };

            // enables only test systems
            //initPaymentRequest.InTestMode();

            var response = await client.InitPaymentAsync(initPaymentRequest);

            // open browser = selenium can be here ^)
            Assert.NotNull(response);
            Assert.NotNull(response.RedirectUrl);
            Browser.Open(response.RedirectUrl);

            // we have some time to manually finish payment.
            var request = _server.WaitForRequest(TimeSpan.FromMinutes(3));
            _output.WriteLine(request.Uri.AbsoluteUri);

            var resultUrl = client.ResultUrl.Parse(request.Uri);

            // to return money back - it's enough to reject payment
            // and hope that your payment service supports it.
            var resultUrlResponse = client.ResultUrl.TryReturnReject(resultUrl, "sorry, my bad...");
            _output.WriteLine(resultUrlResponse.Content);

            request.SendResponse(resultUrlResponse.Content);
        }
    }
}