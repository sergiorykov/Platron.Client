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

        public static SignedValues Empty => new SignedValues();
    }

    internal sealed class SignatureValueProvider
    {
        private const string SignatureFieldName = "pg_sig";
        private readonly IXmlSerializer _serializer;

        public SignatureValueProvider(IXmlSerializer serializer)
        {
            Ensure.ArgumentNotNull(serializer, nameof(serializer));

            _serializer = serializer;
        }

        public List<string> GetValuesToSign(object value)
        {
            Ensure.ArgumentNotNull(value, nameof(value));

            // Need to serialize plain object to order signing values by their xml names.
            var xml = _serializer.Serialize(value, "anyroot");
            return GetSignedValuesCore(xml).Values;
        }

        public SignedValues GetSignedValues(string xml)
        {
            Ensure.ArgumentNotNullOrEmptyString(xml, "xml");

            try
            {
                return GetSignedValuesCore(xml);
            }
            catch (Exception)
            {
                return SignedValues.Empty;
            }
        }

        private SignedValues GetSignedValuesCore(string xml)
        {
            var document = XDocument.Parse(xml);
            if (document.Root == null)
            {
                return SignedValues.Empty;
            }

            var values = document.Root
                .Elements()
                .Where(x => x.Name.LocalName != SignatureFieldName)
                .OrderBy(x => x.Name.LocalName)
                .SelectMany(GetElementValues)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            var signature = document.Root.Element(SignatureFieldName)?.Value;

            return new SignedValues
                   {
                       Values = values,
                       Signature = signature
                   };
        }

        private static IEnumerable<string> GetElementValues(XElement element)
        {
            if (!element.HasElements)
            {
                return new[] {element.Value};
            }

            return element.Elements()
                .OrderBy(x => x.Name.LocalName)
                .SelectMany(GetElementValues);
        }
    }
}