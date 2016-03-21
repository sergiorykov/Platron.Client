using Platron.Client.Extensions;
using Platron.Client.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Platron.Client.Tests
{
    public sealed class InitPaymentTests
    {
        [Theory]
        [InlineData("https://platrondoesnotlivehere.com", "DNS cann't resolve")]
        [InlineData("http://google.com:3434", "Valid address, but service not available")]
        public async Task InitPayment_PlatronNotAvailableOrNotResolvable_ThrowsServiceNotAvailable(
            string notAvailableUrl, string description)
        {
            var initPayment = new InitPaymentRequest(1.Rub(), "sample description");

            var connection = new Connection(new Uri(notAvailableUrl), new Credentials("0000", "secret"), TimeSpan.FromSeconds(5));
            var client = new PlatronClient(connection);

            await Assert.ThrowsAsync<ServiceNotAvailableApiException>(() => client.InitPaymentAsync(initPayment));
        }

        [Fact]
        public async Task InitPayment_InvalidMerchant_ThrowsInvalidResponse()
        {
            var initPayment = new InitPaymentRequest(1.Rub(), "sample description");
            var client = new PlatronClient("0000", "secret");

            var exception =
                await Assert.ThrowsAsync<ErrorApiException>(() => client.InitPaymentAsync(initPayment));
            Assert.Equal(ErrorCode.InvalidMerchant, exception.Error.Code);
        }

        [Fact]
        public async Task InitPayment_InvalidSecretKey_ThrowsInvalidResponse()
        {
            var initPayment = new InitPaymentRequest(1.Usd(), "sample description");
            var client = new PlatronClient(SettingsStorage.Credentials.MerchantId, "secret");

            var exception =
                await Assert.ThrowsAsync<ErrorApiException>(() => client.InitPaymentAsync(initPayment));
            Assert.Equal(ErrorCode.InvalidSignature, exception.Error.Code);
        }

        [Trait("Category", Categories.Integration)]
        [Fact]
        public async Task InitPayment_ValidMerchant_Succeeds()
        {
            var initPayment = new InitPaymentRequest(1.Rub(), "sample description");
            var client = new PlatronClient(SettingsStorage.Credentials);

            initPayment.InTestMode();
            var response = await client.InitPaymentAsync(initPayment);

            Assert.NotNull(response);
            Assert.NotNull(response.RedirectUrl);
        }

        [Trait("Category", Categories.Integration)]
        [Fact]
        public async Task InitPaymentAsHtml_ValidMerchant_ReturnsHtml()
        {
            var initPayment = new InitPaymentRequest(1.Eur(), "sample description");
            var client = new PlatronClient(SettingsStorage.Credentials);

            initPayment.InTestMode();
            var html = await client.InitPaymentAsHtmlAsync(initPayment);

            // requestUri contains redirect uri: https://www.platron.ru/payment_params.php?customer=f00e1b48ea91013cc7a40242f218e68821586740
            Assert.NotNull(html.RequestUri);

            // and requestUri contains 'customer' which hides inside html too:
            // <input type="hidden" name="customer" value="f00e1b48ea91013cc7a40242f218e68821586740">
            var customer = html.RequestUri.Query.Split('=').Last();
            Assert.True(html.Content.Contains(customer));
        }

        [Fact]
        public async Task InitPaymentAsHtml_InvalidMerchant_ReturnsHtml()
        {
            var initPayment = new InitPaymentRequest(1.Rub(), "sample description");
            initPayment.Language = PlatronLanguage.English; // doesn't work. still in russian

            var client = new PlatronClient("0000", "secret");

            initPayment.InTestMode();
            var html = await client.InitPaymentAsHtmlAsync(initPayment);

            Assert.True(html.Content.Contains("Incorrect merchant"));
        }

        [Trait("Category", Categories.Integration)]
        [Theory]
        [InlineData(HttpRequestEncodingType.Get, InitPaymentResponseType.RedirectLink)]
        [InlineData(HttpRequestEncodingType.Get, InitPaymentResponseType.HtmlForm)]
        [InlineData(HttpRequestEncodingType.PostWithQueryString, InitPaymentResponseType.RedirectLink)]
        [InlineData(HttpRequestEncodingType.PostWithQueryString, InitPaymentResponseType.HtmlForm)]
        [InlineData(HttpRequestEncodingType.PostWithXml, InitPaymentResponseType.RedirectLink)]
        [InlineData(HttpRequestEncodingType.PostWithXml, InitPaymentResponseType.HtmlForm)]
        public async Task InitPayment_SpecifiedRequestEncodingAndReturnType_Succeeds(
            HttpRequestEncodingType encodingType, InitPaymentResponseType responseType)
        {
            var connection = new Connection(PlatronClient.PlatronUrl, SettingsStorage.Credentials, encodingType);
            var client = new PlatronClient(connection);
            // To find out what really happens enable proxy (thru fiddler)
            // and use custom connection over http to watch plain requests
            //.EnableProxy(new WebProxy("http://localhost:8888", false));

            var initPayment = new InitPaymentRequest(1.Rub(), "Money first");
            initPayment.InTestMode();
            initPayment.OrderId = Guid.NewGuid().ToString("N");

            switch (responseType)
            {
                case InitPaymentResponseType.RedirectLink:
                    var response = await client.InitPaymentAsync(initPayment);
                    Assert.NotNull(response);
                    break;
                case InitPaymentResponseType.HtmlForm:
                    var html = await client.InitPaymentAsHtmlAsync(initPayment);
                    Assert.NotNull(html);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(responseType), responseType, null);
            }
        }
    }
}