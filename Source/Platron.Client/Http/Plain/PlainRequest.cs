using System.Xml.Serialization;

namespace Platron.Client.Http.Plain
{
    public abstract class PlainRequest
    {
        [XmlElement("pg_merchant_id")]
        public string MerchantId { get; set; }

        [XmlElement("pg_salt")]
        public string Salt { get; set; }

        [XmlElement("pg_sig")]
        public string Signature { get; set; }

        [XmlElement("pg_testing_mode", IsNullable = false)]
        public string TestingMode { get; set; }
    }
}