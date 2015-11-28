using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Platron.Client.Authentication;
using Platron.Client.Http.Callbacks;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public class Connection : IConnection
    {
        private static readonly HashSet<ErrorCode> errorCodesWithoutSignature = new HashSet<ErrorCode>(
            new[] {ErrorCode.InvalidMerchant, ErrorCode.InvalidSignature, ErrorCode.BadRequestParameter});

        private readonly Authenticator _authenticator;
        private readonly HttpRequestEncoder _httpRequestEncoder;
        private readonly IXmlPipeline _xmlPipeline;
        private HttpClient _httpClient;

        public Connection(Credentials credentials) : this(PlatronClient.PlatronUrl, credentials)
        {
        }

        public Connection(Uri baseAddress, Credentials credentials, HttpRequestEncodingType httpRequestEncodingType = HttpRequestEncodingType.PostWithXml)
            : this(baseAddress, credentials, TimeSpan.FromSeconds(30), new XmlPipeline(), httpRequestEncodingType)
        {
        }


        public Connection(Uri baseAddress, Credentials credentials, TimeSpan timeout, HttpRequestEncodingType httpRequestEncodingType = HttpRequestEncodingType.PostWithXml)
            : this(baseAddress, credentials, timeout, new XmlPipeline(), httpRequestEncodingType)
        {
        }

        public Connection(Uri baseAddress, Credentials credentials, TimeSpan timeout, IXmlPipeline xmlPipeline, HttpRequestEncodingType httpRequestEncodingType = HttpRequestEncodingType.PostWithXml)
        {
            Ensure.ArgumentNotNull(baseAddress, "baseAddress");
            Ensure.ArgumentNotNull(credentials, "credentials");
            Ensure.ArgumentNotNull(xmlPipeline, "xmlPipeline");

            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The base address '{0}' must be an absolute URI",
                        baseAddress), nameof(baseAddress));
            }

            _xmlPipeline = xmlPipeline;
            _authenticator = new Authenticator(credentials, xmlPipeline);
            _httpRequestEncoder = new HttpRequestEncoder(xmlPipeline, httpRequestEncodingType);

            _httpClient = new HttpClient
                          {
                              BaseAddress = baseAddress,
                              Timeout = timeout
                          };

            BaseAddress = baseAddress;
            Callback = new CallbackResponder(_authenticator, _xmlPipeline);
        }

        public Uri BaseAddress { get; }

        public HttpRequestEncodingType EncodingType => _httpRequestEncoder.EncodingType;

        public ICallbackResponder Callback { get; }

        public Connection EnableProxy(WebProxy proxy)
        {
            Ensure.ArgumentNotNull(proxy, "proxy");

            var handler = new HttpClientHandler
                          {
                              Proxy = proxy,
                              UseProxy = true
                          };

            var httpClient = new HttpClient(handler) {BaseAddress = BaseAddress};
            Interlocked.Exchange(ref _httpClient, httpClient);

            return this;
        }

        public async Task<IApiResponse<TPlainResponse>> SendAsync<TPlainResponse>(Uri uri, ClientRequest request)
            where TPlainResponse : PlainResponse, new()
        {
            Ensure.ArgumentNotNull(uri, "uri");
            Ensure.ArgumentNotNull(request, "request");

            var apiRequest = new ApiRequest(uri, request.GetPlain());
            _authenticator.Apply(apiRequest);

            var response = await ExecuteAsync(apiRequest).ConfigureAwait(false);
            if (!response.IsXml())
            {
                throw new InvalidResponseApiException($"Expected xml content but received '{response.ContentType}'",
                    response);
            }

            HandleErrors(response);
            if (!_authenticator.Satisfies(response))
            {
                throw new InvalidResponseApiException("Response signature is invalid", response);
            }

            var apiResponse = _xmlPipeline.Deserialize<TPlainResponse>(response);
            return apiResponse;
        }

        public async Task<IApiResponse<string>> SendAsync(Uri uri, ClientRequest request)
        {
            Ensure.ArgumentNotNull(uri, "uri");
            Ensure.ArgumentNotNull(request, "request");

            var apiRequest = new ApiRequest(uri, request.GetPlain());
            _authenticator.Apply(apiRequest);

            var response = await ExecuteAsync(apiRequest).ConfigureAwait(false);
            if (!response.IsHtml())
            {
                throw new InvalidResponseApiException($"Expected html content but received '{response.ContentType}'",
                    response);
            }

            // nothing to handle: It's always OK 200 and all possible errors are hidden inside of html.
            return new ApiResponse<string>(response, response.Body);
        }

        private void HandleErrors(HttpResponse response)
        {
            var errorResponse = _xmlPipeline.Deserialize<PlainErrorWithCodeResponse>(response);
            if (errorResponse.Body.Status == ResponseKnownStatuses.Ok)
            {
                return;
            }

            if (HasSignature(errorResponse.Body) && !_authenticator.Satisfies(response))
            {
                throw new InvalidResponseApiException("Error response signature is invalid", response);
            }

            var error = new PlatronError(errorResponse.Body.ErrorCode, errorResponse.Body.ErrorDescription);
            throw new ErrorApiException(error, response);
        }

        private bool HasSignature(PlainErrorWithCodeResponse response)
        {
            return !errorCodesWithoutSignature.Contains((ErrorCode) response.ErrorCode);
        }

        private async Task<HttpResponse> ExecuteAsync(ApiRequest apiRequest)
        {
            try
            {
                var httpRequest = _httpRequestEncoder.Encode(apiRequest);

                var message = await _httpClient
                    .SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead)
                    .ConfigureAwait(false);

                var body = await message.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);

                var contentType = GetContentMediaType(message.Content);

                return new HttpResponse(
                    message.StatusCode,
                    body,
                    message.RequestMessage.RequestUri,
                    contentType,
                    message.Headers.ToDictionary(h => h.Key, h => h.Value.First()));
            }
            catch (HttpRequestException e)
            {
                var message = e.Message;
                if (e.InnerException is WebException)
                {
                    message = e.InnerException.Message;
                }

                throw new ServiceNotAvailableApiException(message, e);
            }
            catch (Exception e)
            {
                throw new ServiceNotAvailableApiException(e.Message, e);
            }
        }

        private static string GetContentMediaType(HttpContent httpContent)
        {
            return httpContent.Headers?.ContentType?.MediaType;
        }
    }
}