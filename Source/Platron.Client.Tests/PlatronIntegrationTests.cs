using System;
using System.Linq;
using System.Threading.Tasks;
using Platron.Client.Extensions;
using Platron.Client.Http;
using Xunit;

namespace Platron.Client.Tests
{
    public sealed class PlatronIntegrationTests
    {
        [Theory]
        [InlineData("https://platrondoesnotlivehere.com", "DNS cann't resolve")]
        [InlineData("http://google.com:3434", "Valid address, but service not available")]
        public async Task InitPayment_PlatronNotAvailableOrNotResolvable_ThrowsServiceNotAvailable(
            string notAvailableUrl, string description)
        {
            var initPayment = new InitPaymentRequest(1.Rur(), "sample description");

            var connection = new Connection(new Uri(notAvailableUrl), new Credentials("0000", "secret"));
            var client = new PlatronClient(connection);

            await Assert.ThrowsAsync<ServiceNotAvailableApiException>(() => client.InitPaymentAsync(initPayment));
        }

        [Fact]
        public async Task InitPayment_InvalidMerchant_ThrowsInvalidResponse()
        {
            var initPayment = new InitPaymentRequest(1.Rur(), "sample description");
            var client = new PlatronClient("0000", "secret");

            var exception =
                await Assert.ThrowsAsync<ErrorApiException>(() => client.InitPaymentAsync(initPayment));
            Assert.Equal(ErrorCode.InvalidMerchant, exception.Error.Code);
        }

        [Fact]
        public async Task InitPayment_InvalidSecretKey_ThrowsInvalidResponse()
        {
            var initPayment = new InitPaymentRequest(1.Rur(), "sample description");
            var client = new PlatronClient(SettingsStorage.Credentials.MerchantId, "secret");

            var exception =
                await Assert.ThrowsAsync<ErrorApiException>(() => client.InitPaymentAsync(initPayment));
            Assert.Equal(ErrorCode.InvalidSignature, exception.Error.Code);
        }

        [Fact]
        public async Task InitPayment_ValidMerchant_Succeeds()
        {
            var initPayment = new InitPaymentRequest(1.Usd(), "sample description");
            var client = new PlatronClient(SettingsStorage.Credentials);

            initPayment.InTestMode();
            var response = await client.InitPaymentAsync(initPayment);

            Assert.NotNull(response);
            Assert.NotNull(response.RedirectUrl);
        }

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
            var initPayment = new InitPaymentRequest(1.Rur(), "sample description");
            initPayment.Language = PlatronLanguage.English; // doesn't work. still in russian

            var client = new PlatronClient("0000", "secret");

            initPayment.InTestMode();
            var html = await client.InitPaymentAsHtmlAsync(initPayment);

            Assert.True(html.Content.Contains("Incorrect merchant"));
        }

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
            //.EnableProxy(new WebProxy("http://localhost:8888", false));

            var initPayment = new InitPaymentRequest(1.Rur(), "Утром деньги, вечером стулья");
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

        //
        //            Uri callback = new Uri("http://smartairkey.tk:8081/platron/result?pg_salt=49c1e&pg_order_id=e76346a58c9c4c44bfa4e2f8600d9215&pg_payment_id=21404388&pg_amount=1.0000&pg_currency=RUR&pg_net_amount=0.94&pg_ps_amount=1&pg_ps_full_amount=1.00&pg_ps_currency=RUR&pg_payment_system=YANDEXMONEY&pg_result=1&pg_payment_date=2015-10-28+00%3A16%3A09&pg_can_reject=1&pg_user_phone=79261238736&pg_need_phone_notification=1&pg_sig=b6d3e98ff7ead152dcf8c3b1da9a15cb");
        //            //Uri callback = new Uri("http://localhost:8081/platron/result?pg_salt=0bd68e&pg_order_id=654&pg_payment_id=765432&pg_amount=100.0000&pg_currency=RUR&pg_net_amount=100.00&pg_ps_amount=105.00&pg_ps_full_amount=105.00&pg_ps_currency=RUR&pg_payment_system=INPLATMTS&pg_result=1&pg_payment_date=2008-12-30+23:59:30&pg_can_reject=0&pg_user_phone=79818244116&pg_need_phone_notification=1&pg_user_contact_email=test@test.ru&pg_need_email_notification=1&uservar1=45363456&pg_sig=da61f9d237952f56bd05c602d28780b3");
        //        {
        //        public void ResultUrl_ValidResponse_SuccessfullyParsed()
        //        [Fact]

        //
        //            ResultUrl resultUrl;
        //            bool parsed = ResultUrl.TryParse(callback, out resultUrl);
        //
        //            Assert.True(parsed);
        //        }
    }
}