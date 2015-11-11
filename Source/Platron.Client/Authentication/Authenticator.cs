using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Platron.Client.Extensions;
using Platron.Client.Http;
using Platron.Client.Serializers;
using Platron.Client.Utils;

namespace Platron.Client.Authentication
{
    internal sealed class Authenticator
    {
        private readonly SignatureValueProvider _valueProvider;

        public Authenticator(Credentials credentials, IXmlSerializer serializer)
        {
            Ensure.ArgumentNotNull(credentials, "credentials");
            Credentials = credentials;

            _valueProvider = new SignatureValueProvider(serializer);
        }

        public Credentials Credentials { get; }

        public void Apply(ApiRequest request)
        {
            request.Plain.MerchantId = Credentials.MerchantId;
            request.Plain.Salt = SaltProvider.Generate();

            var scriptPath = request.Endpoint.GetScriptPath();
            var values = _valueProvider.GetValuesToSign(request.Plain);

            request.Plain.Signature = Sign(scriptPath, values);
        }

        public bool Satisfies(IHttpResponse response)
        {
            var values = _valueProvider.GetSignedValues(response.Body);
            var scriptPath = response.RequestUri.GetScriptPath();
            var signature = Sign(scriptPath, values.Values);

            return signature == values.Signature;
        }

        private string Sign(string scriptPath, IEnumerable<string> values)
        {
            var signingValues = new List<string>();
            signingValues.Add(scriptPath);
            signingValues.AddRange(values);
            signingValues.Add(Credentials.SecretKey);

            var content = string.Join(";", signingValues);
            var rawContent = Encoding.UTF8.GetBytes(content);
            using (var md5 = MD5.Create())
            {
                var rawSignature = md5.ComputeHash(rawContent);
                var signature = Hex(rawSignature);
                return signature;
            }
        }

        private static string Hex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}