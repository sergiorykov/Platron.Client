using System.Xml.Serialization;

namespace Platron.Client.Http.Plain
{
    public abstract class PlainResponse
    {
        [XmlElement("pg_status")]
        public string Status { get; set; }

        [XmlElement("pg_salt")]
        public string Salt { get; set; }

        [XmlElement("pg_sig")]
        public string Signature { get; set; }
    }
}