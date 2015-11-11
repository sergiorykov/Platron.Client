using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public sealed class XmlPipeline : IXmlPipeline
    {
        public string Serialize(object value, string root)
        {
            Ensure.ArgumentNotNull(value, "value");

            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add("", "");

            var serializer = new XmlSerializer(value.GetType(), new XmlRootAttribute(root ?? "root"));
            using (TextWriter writer = new EncodingStringWriter(Encoding.UTF8))
            {
                serializer.Serialize(writer, value, emptyNamespace);
                return writer.ToString();
            }
        }

        public IApiResponse<TPlainResponse> Deserialize<TPlainResponse>(IHttpResponse response)
            where TPlainResponse : PlainResponse
        {
            Ensure.ArgumentNotNull(response, "response");

            try
            {
                var serializer = new XmlSerializer(typeof (TPlainResponse), new XmlRootAttribute("response"));
                using (var reader = new StringReader(response.Body))
                {
                    var plain = (TPlainResponse) serializer.Deserialize(reader);
                    return new ApiResponse<TPlainResponse>(response, plain);
                }
            }
            catch (Exception)
            {
                throw new InvalidResponseApiException("Expected valid xml", response);
            }
        }

        public string Serialize(ApiRequest request)
        {
            return Serialize(request.Plain, "request");
        }

        /// <summary>
        ///     Need to subclass StringWriter in order to override Encoding.
        /// </summary>
        internal sealed class EncodingStringWriter : StringWriter
        {
            public EncodingStringWriter(Encoding encoding)
            {
                Encoding = encoding;
            }

            public override Encoding Encoding { get; }
        }
    }
}