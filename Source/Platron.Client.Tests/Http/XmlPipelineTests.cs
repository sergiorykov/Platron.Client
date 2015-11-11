using Moq;
using Platron.Client.Http;
using Platron.Client.Http.Plain;
using Xunit;

namespace Platron.Client.Tests.Http
{
    public sealed class XmlPipelineTests
    {
        [Fact]
        public void XmlPipeline_ErrorResponse_Succeeds()
        {
            var content =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<response><pg_salt>88a8df6f7c7049c2b248159faf5c9673</pg_salt><pg_status>error</pg_status><pg_error_code>101</pg_error_code><pg_error_description>Incorrect merchant</pg_error_description></response>\n";

            var response = Mock.Of<IHttpResponse>(x => x.Body == content);

            var pipeline = new XmlPipeline();
            var apiResponse = pipeline.Deserialize<PlainErrorResponse>(response);
            Assert.Equal("error", apiResponse.Body.Status);
        }
    }
}