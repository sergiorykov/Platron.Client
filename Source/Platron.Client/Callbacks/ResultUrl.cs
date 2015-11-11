//using System;
//using System.Collections.Specialized;
//using System.Linq;
//using Platron.Client.Exceptions;
//using Platron.Client.Extensions;
//using Platron.Client.Http;
//
//namespace Platron.Client.Callbacks
//{
//    public enum PaymentRejectType
//    {
//        Unrevokable = 0,
//        CanBeRolledBack = 1
//    }
//
//    public enum CardClearingBehaviourType
//    {
//        StoreMustSendCommand = 0,
//        WaitForPlatronWillSend = 1
//    }
//
//    public abstract class PlatronCallback
//    {
//        protected PlatronCallback(PlatronResponse1 response)
//        {
//            Response = response;
//            Parameters = Response.ToCollection();
//        }
//
//        protected PlatronResponse1 Response { get; set; }
//        public NameValueCollection Parameters { get; }
//        public Uri From { get; }
//
//        public bool VerifySignature(Credentials key)
//        {
//            return Response.VerifySignature(key);
//        }
//    }
//
//    public sealed class ResultUrl : PlatronCallback
//    {
//        private ResultUrl(Uri address) : base(new PlatronResponse1(address))
//        {
//        }
//
//        private ResultUrl(PlatronResponse1 response) : base(response)
//        {
//        }
//
//        public string OrderId { get; private set; }
//        public string PaymentId { get; private set; }
//        public PlatronPaymentCurrency Currency { get; private set; }
//        public double Amount { get; private set; }
//        public double NetAmount { get; private set; }
//        public double PsAmount { get; private set; }
//        public double PsFullAmount { get; private set; }
//        public PlatronPaymentCurrency PsCurrency { get; private set; }
//        public double Overpayment { get; private set; }
//        public string PaymentSystem { get; private set; }
//        public bool Result { get; private set; }
//        public DateTime PaymentDate { get; private set; }
//        public PaymentRejectType CanReject { get; private set; }
//        public string CardBrand { get; private set; }
//        public string CardPan { get; private set; }
//        public string CardHash { get; private set; }
//        public string AuthCode { get; private set; }
//        public CardClearingBehaviourType Captured { get; private set; }
//        public string UserPhone { get; private set; }
//        public bool NeedPhoneNotification { get; private set; }
//        public string UserContactEmail { get; private set; }
//        public bool NeedEmailNotification { get; private set; }
//        public int FailureCode { get; private set; }
//        public string FailureDescription { get; private set; }
//        public string RecurringProfileId { get; private set; }
//        public DateTime RecurringProfileExpiryDate { get; private set; }
//
//        public static bool TryParse(Uri value, out ResultUrl resultUrl)
//        {
//            try
//            {
//                resultUrl = Parse(value);
//                return true;
//            }
//            catch (Exception)
//            {
//                resultUrl = new ResultUrl(value);
//                return false;
//            }
//        }
//
//        public static ResultUrl Parse(Uri value)
//        {
//            PlatronResponse1 platronResponse;
//            if (!PlatronResponse1.TryParse(value, out platronResponse))
//            {
//                throw new InvalidArgumentPlatronException(nameof(value), "Parse uri failed");
//            }
//
//            var resultUrl = new ResultUrl(platronResponse);
//            resultUrl.Result = platronResponse.GetBool("pg_result", x => x == "1");
//
//            if (!resultUrl.Result)
//            {
//                resultUrl.FailureCode = platronResponse.GetOrDefault("pg_failure_code", 0);
//                resultUrl.FailureDescription = platronResponse.GetOrDefault("pg_failure_description", string.Empty);
//
//                return resultUrl;
//            }
//
//            resultUrl.OrderId = platronResponse.GetOrDefault("pg_order_id", string.Empty);
//
//            resultUrl.PaymentId = platronResponse.Get("pg_payment_id");
//            resultUrl.Amount = platronResponse.Get<double>("pg_amount");
//            resultUrl.Currency = platronResponse.Get<PlatronPaymentCurrency>("pg_currency");
//            resultUrl.NetAmount = platronResponse.Get<double>("pg_net_amount");
//            resultUrl.PsAmount = platronResponse.Get<double>("pg_ps_amount");
//            resultUrl.PsFullAmount = platronResponse.Get<double>("pg_ps_full_amount");
//            resultUrl.PsCurrency = platronResponse.Get<PlatronPaymentCurrency>("pg_ps_currency");
//            resultUrl.Overpayment = platronResponse.GetOrDefault<double>("pg_overpayment", 0);
//
//            resultUrl.PaymentSystem = platronResponse.Get("pg_payment_system");
//            resultUrl.PaymentDate = platronResponse.GetDate("pg_payment_date");
//            resultUrl.CanReject = platronResponse.GetOrDefault("pg_can_reject", PaymentRejectType.Unrevokable);
//
//            if (platronResponse.Contains("pg_card_brand"))
//            {
//                resultUrl.CardBrand = platronResponse.GetOrDefault("pg_card_brand", string.Empty);
//                resultUrl.CardPan = platronResponse.GetOrDefault("pg_card_pan", string.Empty);
//                resultUrl.CardHash = platronResponse.GetOrDefault("pg_card_hash", string.Empty);
//                resultUrl.AuthCode = platronResponse.GetOrDefault("pg_auth_code", string.Empty);
//                resultUrl.Captured = platronResponse.GetOrDefault("pg_captured",
//                    CardClearingBehaviourType.WaitForPlatronWillSend);
//            }
//
//            resultUrl.UserPhone = platronResponse.GetOrDefault("pg_user_phone", string.Empty);
//            resultUrl.NeedPhoneNotification = platronResponse.GetBool("pg_need_phone_notification", x => x == "1");
//
//            resultUrl.UserContactEmail = platronResponse.GetOrDefault("pg_user_contact_email", string.Empty);
//            resultUrl.NeedEmailNotification = platronResponse.GetBoolOrDefault("pg_need_email_notification",
//                x => x == "1", false);
//
//            resultUrl.RecurringProfileId = platronResponse.GetOrDefault("pg_recurring_profile_id", string.Empty);
//            resultUrl.RecurringProfileExpiryDate = platronResponse.GetDateOrDefault("pg_recurring_profile_expiry_date",
//                DateTime.MinValue);
//
//            return resultUrl;
//        }
//
//        public sealed class Return
//        {
//            private readonly HttpRequestEncoder _request;
//
//            public Return(HttpRequestEncoder request)
//            {
//                _request = request;
//            }
//
//            public static Return TryReject(ResultUrl resultUri, string description, Credentials key)
//            {
//                if (resultUri.CanReject == PaymentRejectType.Unrevokable)
//                {
//                    return Error(resultUri.From, description, key);
//                }
//
////                var address = resultUri.From;
////                var request = new HttpRequestEncoder(address)
//////                    .Add("pg_status", "rejected")
//////                    .Add("pg_description", description)
////                    .Sign(key);
//
//                return new Return(null);
//            }
//
//            public static Return Error(Uri resultUri, string description, Credentials key)
//            {
////                var request = new HttpRequestEncoder(resultUri)
//////                    .Add("pg_status", "error")
//////                    .Add("pg_description", description)
//////                    .Add("pg_error_description", description)
////                    .Sign(key);
//
//                return new Return(null);
//            }
//
//            public static Return Ok(Uri resultUri, Credentials key)
//            {
////                var address = resultUri.AbsolutePath;
////                var request = new HttpRequestEncoder(resultUri)
//////                    .Add("pg_status", "ok")
////                    .Sign(key);
//
//                return new Return(null);
//            }
//
//            public string Encode()
//            {
////                return _request.ToXml();
//                return string.Empty;
//            }
//        }
//    }
//}

