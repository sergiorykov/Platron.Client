using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public sealed class HttpRequestEncoder
    {
        private readonly IXmlPipeline _xmlPipeline;

        public HttpRequestEncoder(IXmlPipeline xmlPipeline,
            HttpRequestEncodingType encodingType = HttpRequestEncodingType.PostWithXml)
        {
            Ensure.ArgumentNotNull(xmlPipeline, "xmlPipeline");

            _xmlPipeline = xmlPipeline;
            EncodingType = encodingType;
        }

        public HttpRequestEncodingType EncodingType { get; }

        public HttpRequestMessage Encode(ApiRequest request)
        {
            var message = new HttpRequestMessage();

            switch (EncodingType)
            {
                case HttpRequestEncodingType.Get:
                    message.Method = HttpMethod.Get;
                    message.RequestUri = new Uri($"{request.Endpoint}?{GetQueryString(request)}", UriKind.Relative);
                    return message;
                case HttpRequestEncodingType.PostWithQueryString:
                    message.Method = HttpMethod.Post;
                    message.RequestUri = new Uri($"{request.Endpoint}?{GetQueryString(request)}", UriKind.Relative);
                    return message;
                case HttpRequestEncodingType.PostWithXml:
                    message.Method = HttpMethod.Post;
                    message.RequestUri = request.Endpoint;
                    message.Content = new FormUrlEncodedContent(
                        new[] {new KeyValuePair<string, string>("pg_xml", _xmlPipeline.Serialize(request))});
                    return message;
                default:
                    throw new ArgumentOutOfRangeException(nameof(EncodingType), EncodingType, "Encoding not implemented");
            }
        }

        private string GetQueryString(ApiRequest request)
        {
            string xml = _xmlPipeline.Serialize(request);
            List<KeyValuePair<string, string>> values = GetQueryStringValues(xml);

            string query;
            using (var content = new FormUrlEncodedContent(values))
            {
                query = content.ReadAsStringAsync().Result;
            }

            return query;
        }

        private static List<KeyValuePair<string, string>> GetQueryStringValues(string xml)
        {
            var document = XDocument.Parse(xml);
            if (document.Root == null)
            {
                return new List<KeyValuePair<string, string>>();
            }

            var values = document.Root
                .Elements()
                .SelectMany(element =>
                {
                    if (!element.HasElements)
                    {
                        return new[]
                               {
                                   new KeyValuePair<string, string>(element.Name.LocalName, element.Value.ToString())
                               };
                    }

                    return element.Elements()
                        .Select(child =>
                        {
                            var name = string.Format(
                                CultureInfo.InvariantCulture,
                                "{0}[{1}]",
                                element.Name.LocalName,
                                child.Name.LocalName);

                            return new KeyValuePair<string, string>(name, element.Value.ToString());
                        });
                })
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .ToList();

            return values;
        }
    }
}