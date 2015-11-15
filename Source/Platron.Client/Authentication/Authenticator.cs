using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Platron.Client.Extensions;
using Platron.Client.Http;
using Platron.Client.Http.Callbacks;
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

            string scriptPath = request.Endpoint.GetScriptPath();
            List<string> values = _valueProvider.GetValuesToSign(request.Plain);

            request.Plain.Signature = Sign(scriptPath, values);
        }

        public void Apply(ApiCallbackResponse response)
        {
            response.Plain.Salt = SaltProvider.Generate();

            string scriptPath = response.Endpoint.GetScriptPath();
            List<string> values = _valueProvider.GetValuesToSign(response.Plain);

            response.Plain.Signature = Sign(scriptPath, values);
        }

        public bool Satisfies(CallbackRequest request)
        {
            SignedValues values = _valueProvider.GetSignedValues(request);
            string scriptPath = request.Uri.GetScriptPath();
            string signature = Sign(scriptPath, values.Values);

            return signature == values.Signature;
        }

        public bool Satisfies(IHttpResponse response)
        {
            SignedValues values = _valueProvider.GetSignedValues(response.Body);
            string scriptPath = response.RequestUri.GetScriptPath();
            string signature = Sign(scriptPath, values.Values);

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
                byte[] rawSignature = md5.ComputeHash(rawContent);
                string signature = Hex(rawSignature);
                return signature;
            }
        }

        private static string Hex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty).ToLowerInvariant();
        }
    }
}