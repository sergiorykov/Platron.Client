using System;
using Platron.Client.Authentication;
using Platron.Client.Utils;

namespace Platron.Client.Http.Callbacks
{
    internal sealed class CallbackResponder : ICallbackResponder
    {
        private readonly Authenticator _authenticator;
        private readonly IXmlPipeline _xmlPipeline;

        public CallbackResponder(Authenticator authenticator, IXmlPipeline xmlPipeline)
        {
            Ensure.ArgumentNotNull(authenticator, nameof(authenticator));
            Ensure.ArgumentNotNull(xmlPipeline, nameof(xmlPipeline));

            _authenticator = authenticator;
            _xmlPipeline = xmlPipeline;
        }

        public CallbackRequest Parse(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = CallbackRequest.Parse(uri);
            if (!_authenticator.Satisfies(response))
            {
                throw new InvalidCallbackApiException("Signature is invalid", uri);
            }

            var isSucceded = response.GetBool("pg_result", x => x == "1");
            if (!isSucceded)
            {
                var failureCode = response.GetOrDefault("pg_failure_code", 0);
                var failureDescription = response.GetOrDefault("pg_failure_description", string.Empty);

                throw new ErrorCallbackApiException(uri, new PlatronError(failureCode, failureDescription));
            }

            return response;
        }

        public CallbackResponse EncodeResponse(ApiCallbackResponse response)
        {
            _authenticator.Apply(response);
            var content = _xmlPipeline.Serialize(response);
            return new CallbackResponse {Content = content};
        }
    }

    public interface ICallbackResponder
    {
        CallbackRequest Parse(Uri uri);
        CallbackResponse EncodeResponse(ApiCallbackResponse response);
    }
}