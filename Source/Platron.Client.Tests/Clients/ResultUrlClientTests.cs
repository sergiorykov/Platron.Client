using Xunit;

namespace Platron.Client.Tests.Clients
{
    public sealed class ResultUrlClientTests
    {
        [Fact]
        public void ResultUrl_ReturnOk_ValidXml()
        {
            var client = new PlatronClient(Samples.Credentials);
            var resultUrl = client.ResultUrl.Parse(Samples.ResultUrlWithValidSignature);

            var ok = client.ResultUrl.ReturnOk(resultUrl, "all done");
            Assert.NotNull(ok);
            Assert.NotNull(ok.Content);
        }

        [Fact]
        public void ResultUrl_ValidUri_Succeeds()
        {
            var client = new PlatronClient(Samples.Credentials);
            client.ResultUrl.Parse(Samples.ResultUrlWithValidSignature);
        }
    }
}