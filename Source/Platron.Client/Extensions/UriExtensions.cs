using System;
using System.Linq;
using Platron.Client.Utils;

namespace Platron.Client.Extensions
{
    internal static class UriExtensions
    {
        public static string GetScriptPath(this Uri value)
        {
            Ensure.ArgumentNotNull(value, "value");

            var uri = value.IsAbsoluteUri
                ? value
                : new Uri(new Uri("http://test.me"), value.ToString());

            return uri.LocalPath.Split('/').Last().ToLowerInvariant();
        }
    }
}