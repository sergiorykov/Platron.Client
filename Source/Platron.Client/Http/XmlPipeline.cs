using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    /// <summary>
    /// Implements all xml serialization/deserialization operations.
    /// </summary>
    public sealed class XmlPipeline : IXmlPipeline
    {
        private static readonly string RequestRoot = "request";
        private static readonly string ResponseRoot = "response";

        public string Serialize(object value, string root)
        {
            Ensure.ArgumentNotNull(value, nameof(value));
            Ensure.ArgumentNotNull(root, nameof(root));

            // Need to exclude namespaces from result xml.
            var emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add(string.Empty, string.Empty);

            var serializer = new XmlSerializer(value.GetType(), new XmlRootAttribute(root));
            using (TextWriter writer = new EncodingStringWriter(Encoding.UTF8))
            {
                serializer.Serialize(writer, value, emptyNamespace);
                return writer.ToString();
            }
        }

        public IApiResponse<TPlainResponse> Deserialize<TPlainResponse>(IHttpResponse response)
            where TPlainResponse : PlainResponse
        {
            Ensure.ArgumentNotNull(response, nameof(response));

            try
            {
                var serializer = new XmlSerializer(typeof (TPlainResponse), ResponseRoot);
                using (var reader = new StringReader(response.Body))
                {
                    var plain = (TPlainResponse) serializer.Deserialize(reader);
                    return new ApiResponse<TPlainResponse>(response, plain);
                }
            }
            catch (Exception)
            {
                throw new InvalidResponseApiException("Xml content in response cann't be decoded", response);
            }
        }

        public string Serialize(ApiRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            return Serialize(request.Plain, RequestRoot);
        }

        public string Serialize(ApiCallbackResponse request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            return Serialize(request.Plain, ResponseRoot);
        }

        /// <summary>
        ///     Need to subclass StringWriter in order to override Encoding.
        /// </summary>
        private sealed class EncodingStringWriter : StringWriter
        {
            public EncodingStringWriter(Encoding encoding)
            {
                Encoding = encoding;
            }

            public override Encoding Encoding { get; }
        }
    }
}