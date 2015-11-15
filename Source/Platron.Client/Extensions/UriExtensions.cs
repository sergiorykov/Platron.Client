using System;
using System.Collections.Specialized;
using System.Linq;
using Platron.Client.Utils;

namespace Platron.Client.Extensions
{
    internal static class UriExtensions
    {
        /// <summary>
        /// Returns script path from uri. It's last part of path before query string starts with '?'.
        /// </summary>
        /// <param name="value">Uri.</param>
        /// <returns>Script path.</returns>
        public static string GetScriptPath(this Uri value)
        {
            Ensure.ArgumentNotNull(value, nameof(value));

            var uri = value.IsAbsoluteUri
                ? value
                : new Uri(new Uri("http://test.me"), value.ToString());

            return uri.LocalPath.Split('/').Last().ToLowerInvariant();
        }

        /// <summary>
        /// Decodes query string.
        /// </summary>
        /// <param name="uri">Uri.</param>
        /// <returns>Key-value dictionary.</returns>
        /// <remarks>See http://stackoverflow.com/questions/659887/get-url-parameters-from-a-string-in-net .</remarks>
        /// <remarks>See also http://stackoverflow.com/questions/602642/server-urlencode-vs-httputility-urlencode .</remarks>
        /// <remarks>See also http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx .</remarks>
        public static NameValueCollection DecodeQueryParameters(this Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            if (uri.Query.Length == 0)
                return new NameValueCollection();

            var nameValues = uri.Query.TrimStart('?')
                            .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(nv => nv.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                            .Select(nv => new {
                                Name = nv[0],
                                Value = nv.Length > 2
                                ? string.Join("=", nv, 1, nv.Length - 1)
                                : (nv.Length > 1 ? nv[1] : "")});

            var result = new NameValueCollection();
            foreach (var nameValue in nameValues)
            {
                string value = Uri.UnescapeDataString(nameValue.Value).Replace('+', ' ');
                result.Add(nameValue.Name, value);
            }

            return result;
        }
    }
}