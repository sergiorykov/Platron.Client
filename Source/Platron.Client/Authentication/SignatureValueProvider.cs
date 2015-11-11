using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Platron.Client.Serializers;
using Platron.Client.Utils;

namespace Platron.Client.Authentication
{
    internal sealed class SignedValues
    {
        public List<string> Values { get; set; } = new List<string>();
        public string Signature { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Signature) && Values != null && Values.Count > 0;

        public static SignedValues Empty => new SignedValues();
    }

    internal sealed class SignatureValueProvider
    {
        private const string SignatureFieldName = "pg_sig";
        private readonly IXmlSerializer _serializer;

        public SignatureValueProvider(IXmlSerializer serializer)
        {
            Ensure.ArgumentNotNull(serializer, "serializer");

            _serializer = serializer;
        }

        public List<string> GetValuesToSign(object value)
        {
            Ensure.ArgumentNotNull(value, "value");

            try
            {
                var xml = _serializer.Serialize(value, "anyroot");
                var document = XDocument.Parse(xml);
                return GetValues(document).Values;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public SignedValues GetSignedValues(string xml)
        {
            Ensure.ArgumentNotNullOrEmptyString(xml, "xml");
            try
            {
                var document = XDocument.Parse(xml);
                return GetValues(document);
            }
            catch (Exception)
            {
                return SignedValues.Empty;
            }
        }

        private SignedValues GetValues(XDocument document)
        {
            if (document.Root == null)
            {
                return SignedValues.Empty;
            }

            var values = document.Root
                .Elements()
                .Where(x => x.Name.LocalName != SignatureFieldName)
                .OrderBy(x => x.Name.LocalName)
                .SelectMany(GetValues)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var signature = document.Root.Element(SignatureFieldName)?.Value;

            return new SignedValues
                   {
                       Values = values,
                       Signature = signature
                   };
        }

        private static IEnumerable<string> GetValues(XElement element)
        {
            if (!element.HasElements)
            {
                return new[] {element.Value};
            }

            return element.Elements()
                .OrderBy(x => x.Name.LocalName)
                .SelectMany(GetValues);
        }
    }
}