using System.Xml.Serialization;

namespace Platron.Client.Http.Plain
{
    public class PlainResponseWithDescription : PlainResponse
    {
        [XmlElement("pg_description", IsNullable = false)]
        public string Description { get; set; }
    }
}