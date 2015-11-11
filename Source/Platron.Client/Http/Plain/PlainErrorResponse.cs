using System.Xml.Serialization;

namespace Platron.Client.Http.Plain
{
    public sealed class PlainErrorResponse : PlainResponse
    {
        [XmlElement("pg_error_code")]
        public int ErrorCode { get; set; } = (int) Client.ErrorCode.None;

        [XmlElement("pg_error_description", IsNullable = true)]
        public string ErrorDescription { get; set; }
    }
}