using System;
using System.Xml.Serialization;
using Platron.Client.Http;
using Platron.Client.Http.Callbacks;
using Platron.Client.Http.Plain;
using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class ResultUrlClient
    {
        private readonly ICallbackResponder _callback;

        public ResultUrlClient(ApiConnection connection)
        {
            Ensure.ArgumentNotNull(connection, nameof(connection));
            Ensure.ArgumentNotNull(connection.Callback, nameof(connection.Callback));

            _callback = connection.Callback;
        }

        public ResultUrlRequest Parse(Uri uri)
        {
            CallbackRequest request = _callback.Parse(uri);
            return ResultUrlRequest.Parse(request);
        }

        public CallbackResponse ReturnOk(ResultUrlRequest request, string description = "")
        {
            var plain = new ResultUrlRequest.OkReturn
                        {
                            Status = ResponseKnownStatuses.Ok,
                            Description = description,
                            Timeout = 300
                        };
            return _callback.EncodeResponse(new ApiCallbackResponse(request.Uri, plain));
        }

        public CallbackResponse ReturnError(ResultUrlRequest request, string description = "")
        {
            var plain = new PlainResponseWithDescription
            {
                Status = ResponseKnownStatuses.Error,
                Description = description
            };
            return _callback.EncodeResponse(new ApiCallbackResponse(request.Uri, plain));
        }

        public CallbackResponse ReturnError(ResultUrlRequest request, PlatronError error)
        {
            Ensure.ArgumentNotNull(error, nameof(error));

            var plain = new PlainErrorWithCodeResponse
            {
                Status = ResponseKnownStatuses.Error,
                ErrorDescription = error.Description,
                ErrorCode = (int)error.Code
            };
            return _callback.EncodeResponse(new ApiCallbackResponse(request.Uri, plain));
        }

        public CallbackResponse ReturnReject(ResultUrlRequest request, string description = "")
        {
            var plain = new PlainResponseWithDescription
            {
                Status = ResponseKnownStatuses.Reject,
                Description = description
            };
            return _callback.EncodeResponse(new ApiCallbackResponse(request.Uri, plain));
        }

        public CallbackResponse TryReturnReject(ResultUrlRequest request, string description = "")
        {
            if (request.CanReject == PaymentRejectType.Unrevokable)
            {
                return ReturnError(request, description);
            }

            return ReturnReject(request, description);
        }
    }

    public enum PaymentRejectType
    {
        Unrevokable = 0,
        CanBeRolledBack = 1
    }

    public enum CardClearingBehaviourType
    {
        StoreMustSendCommand = 0,
        WaitForPlatronWillSend = 1
    }

    public sealed class ResultUrlRequest
    {
        public sealed class OkReturn : PlainResponseWithDescription
        {
            [XmlElement("pg_timeout", IsNullable = false)]
            public int Timeout { get; set; }
        }

        public string OrderId { get; private set; }
        public string PaymentId { get; private set; }
        public PlatronPaymentCurrency Currency { get; private set; }
        public double Amount { get; private set; }
        public double NetAmount { get; private set; }
        public double PsAmount { get; private set; }
        public double PsFullAmount { get; private set; }
        public PlatronPaymentCurrency PsCurrency { get; private set; }
        public double Overpayment { get; private set; }
        public string PaymentSystem { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public PaymentRejectType CanReject { get; private set; }
        public string CardBrand { get; private set; }
        public string CardPan { get; private set; }
        public string CardHash { get; private set; }
        public string AuthCode { get; private set; }
        public CardClearingBehaviourType Captured { get; private set; }
        public string UserPhone { get; private set; }
        public bool NeedPhoneNotification { get; private set; }
        public string UserContactEmail { get; private set; }
        public bool NeedEmailNotification { get; private set; }
        public string RecurringProfileId { get; private set; }
        public DateTime RecurringProfileExpiryDate { get; private set; }

        public Uri Uri { get; private set; }

        public static ResultUrlRequest Parse(CallbackRequest callback)
        {
            Ensure.ArgumentNotNull(callback, nameof(callback));

            var resultUrl = new ResultUrlRequest();

            resultUrl.Uri = callback.Uri;

            resultUrl.OrderId = callback.GetOrDefault("pg_order_id", string.Empty);

            resultUrl.PaymentId = callback.Get("pg_payment_id");
            resultUrl.Amount = callback.Get<double>("pg_amount");
            resultUrl.Currency = callback.Get<PlatronPaymentCurrency>("pg_currency");
            resultUrl.NetAmount = callback.Get<double>("pg_net_amount");
            resultUrl.PsAmount = callback.Get<double>("pg_ps_amount");
            resultUrl.PsFullAmount = callback.Get<double>("pg_ps_full_amount");
            resultUrl.PsCurrency = callback.Get<PlatronPaymentCurrency>("pg_ps_currency");
            resultUrl.Overpayment = callback.GetOrDefault<double>("pg_overpayment", 0);

            resultUrl.PaymentSystem = callback.Get("pg_payment_system");
            resultUrl.PaymentDate = callback.GetDate("pg_payment_date");
            resultUrl.CanReject = callback.GetOrDefault("pg_can_reject", PaymentRejectType.Unrevokable);

            if (callback.Contains("pg_card_brand"))
            {
                resultUrl.CardBrand = callback.GetOrDefault("pg_card_brand", string.Empty);
                resultUrl.CardPan = callback.GetOrDefault("pg_card_pan", string.Empty);
                resultUrl.CardHash = callback.GetOrDefault("pg_card_hash", string.Empty);
                resultUrl.AuthCode = callback.GetOrDefault("pg_auth_code", string.Empty);
                resultUrl.Captured = callback.GetOrDefault("pg_captured", CardClearingBehaviourType.WaitForPlatronWillSend);
            }

            resultUrl.UserPhone = callback.GetOrDefault("pg_user_phone", string.Empty);
            resultUrl.NeedPhoneNotification = callback.GetBool("pg_need_phone_notification", x => x == "1");

            resultUrl.UserContactEmail = callback.GetOrDefault("pg_user_contact_email", string.Empty);
            resultUrl.NeedEmailNotification = callback.GetBoolOrDefault("pg_need_email_notification", x => x == "1", false);

            resultUrl.RecurringProfileId = callback.GetOrDefault("pg_recurring_profile_id", string.Empty);
            resultUrl.RecurringProfileExpiryDate = callback.GetDateOrDefault("pg_recurring_profile_expiry_date", DateTime.MinValue);

            return resultUrl;
        }
    }
}