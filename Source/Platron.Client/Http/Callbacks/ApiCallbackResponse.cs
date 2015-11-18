using System;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client.Http
{
    public sealed class ApiCallbackResponse
    {
        public ApiCallbackResponse(Uri endpoint, PlainResponse plain)
        {
            Ensure.ArgumentNotNull(endpoint, nameof(endpoint));
            Ensure.ArgumentNotNull(plain, nameof(plain));

            Endpoint = endpoint;
            Plain = plain;
        }

        public Uri Endpoint { get; }
        public PlainResponse Plain { get; }
    }
}