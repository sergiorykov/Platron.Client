using System;
using System.Xml.Serialization;
using Platron.Client.Extensions;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class InitPaymentRequest : ClientRequest
    {
        public InitPaymentRequest(PlatronMoney money, string description)
        {
            Ensure.ArgumentNotNull(money, "money");
            Ensure.ArgumentNotNullOrEmptyString(description, "description");

            Money = money;
            Description = description;
        }

        public string OrderId { get; set; }
        public PlatronMoney Money { get; }
        public string Description { get; }
        public string Encoding { get; set; } = "UTF-8";
        public TimeSpan LifeTime { get; set; } = TimeSpan.FromDays(1);
        public PlatronLanguage Language { get; set; } = PlatronLanguage.Russian;
        public string UserPhone { get; set; }
        public bool? NeedUserPhoneNotification { get; set; }
        public string UserContactEmail { get; set; }
        public bool? NeedEmailNotification { get; set; }
        public Uri ResultUrl { get; set; }

        protected override PlainRequest GetPlainCore()
        {
            return new Plain
                   {
                       OrderId = OrderId,
                       Amount = Money.Amount,
                       Currency = Money.Currency.GetDescription(),
                       Description = Description,
                       Encoding = Encoding,
                       LifeTime = LifeTime.ToPlatronTime(),
                       Language = Language.GetDescription(),
                       ResultUrl = ResultUrl?.AbsolutePath,
                       UserPhone = UserPhone,
                       NeedUserPhoneNotification = NeedUserPhoneNotification.ToZeroOrOne(),
                       UserContactEmail = UserContactEmail,
                       NeedEmailNotification = NeedEmailNotification.ToZeroOrOne()
                   };
        }

        public sealed class Plain : PlainRequest
        {
            [XmlElement("pg_order_id", IsNullable = false)]
            public string OrderId { get; set; }

            [XmlElement("pg_amount")]
            public double Amount { get; set; }

            [XmlElement("pg_currency")]
            public string Currency { get; set; }

            [XmlElement("pg_description")]
            public string Description { get; set; }

            [XmlElement("pg_encoding", IsNullable = false)]
            public string Encoding { get; set; }

            [XmlElement("pg_lifetime", IsNullable = false)]
            public string LifeTime { get; set; }

            [XmlElement("pg_language", IsNullable = false)]
            public string Language { get; set; }

            [XmlElement("pg_user_phone", IsNullable = false)]
            public string UserPhone { get; set; }

            [XmlElement("pg_need_phone_notification", IsNullable = false)]
            public string NeedUserPhoneNotification { get; set; }

            [XmlElement("pg_user_contact_email", IsNullable = false)]
            public string UserContactEmail { get; set; }

            [XmlElement("pg_need_email_notification", IsNullable = false)]
            public string NeedEmailNotification { get; set; }

            [XmlElement("pg_result_url", IsNullable = false)]
            public string ResultUrl { get; set; }
        }
    }
}