using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Platron.Client.Extensions;
using Platron.Client.Utils;

namespace Platron.Client.Http.Callbacks
{
    public sealed class CallbackRequest
    {
        public CallbackRequest(Uri uri, NameValueCollection queryString)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(queryString, nameof(queryString));

            Uri = uri;
            QueryString = queryString;
        }

        public Uri Uri { get; }
        public NameValueCollection QueryString { get; }

        public static CallbackRequest Parse(Uri uri)
        {
            var queryString = uri.DecodeQueryParameters();
            return new CallbackRequest(uri, queryString);
        }

        public T GetOrDefault<T>(string key, T defaultValue)
        {
            if (!Contains(key))
            {
                return defaultValue;
            }

            return Get<T>(key);
        }

        public string Get(string key)
        {
            var value = QueryString[key];
            return value;
        }

        public bool GetBool(string key, Func<string, bool> converter)
        {
            var value = QueryString[key];
            return converter(value);
        }

        public bool GetBoolOrDefault(string key, Func<string, bool> converter, bool defaultValue)
        {
            if (!Contains(key))
            {
                return defaultValue;
            }

            return GetBool(key, converter);
        }

        public T Get<T>(string key)
        {
            var value = QueryString[key];

            var converter = TypeDescriptor.GetConverter(typeof (T));
            return (T) converter.ConvertFromInvariantString(value);
        }

        public DateTime GetDateOrDefault(string key, DateTime defaultValue)
        {
            if (!Contains(key))
            {
                return defaultValue;
            }
            return GetDate(key);
        }

        public DateTime GetDate(string key)
        {
            var value = QueryString[key];

            DateTime result;
            if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal, out result))
            {
                return result;
            }

            return DateTime.Parse(value);
        }

        public bool Contains(string key)
        {
            return !string.IsNullOrEmpty(QueryString[key]);
        }
    }
}