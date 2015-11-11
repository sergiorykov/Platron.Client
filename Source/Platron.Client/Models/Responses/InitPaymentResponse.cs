using System;
using System.Xml.Serialization;
using Platron.Client.Http.Plain;

namespace Platron.Client
{
    public sealed class InitPaymentResponse : ClientResponse<InitPaymentResponse.Plain>
    {
        public string PaymentId { get; private set; }
        public Uri RedirectUrl { get; private set; }
        public string RedirectUrlType { get; private set; }

        protected override void InitCore(Plain plain)
        {
            PaymentId = plain.PaymentId;
            RedirectUrl = new Uri(plain.RedirectUrl);
            RedirectUrlType = plain.RedirectUrlType;
        }

        public sealed class Plain : PlainResponse
        {
            [XmlElement("pg_payment_id")]
            public string PaymentId { get; set; }

            [XmlElement("pg_redirect_url")]
            public string RedirectUrl { get; set; }

            [XmlElement("pg_redirect_url_type")]
            public string RedirectUrlType { get; set; }
        }
    }
}