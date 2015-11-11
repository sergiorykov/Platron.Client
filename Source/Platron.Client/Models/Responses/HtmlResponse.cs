using System;
using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class HtmlResponse
    {
        public HtmlResponse(string content, Uri requestUri = null)
        {
            Ensure.ArgumentNotNullOrEmptyString(content, "content");

            Content = content;
            RequestUri = requestUri;
        }

        public string Content { get; }

        public Uri RequestUri { get; }
    }
}