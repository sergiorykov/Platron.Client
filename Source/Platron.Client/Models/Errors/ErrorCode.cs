namespace Platron.Client
{
    public enum ErrorCode
    {
        None = 0,
        InvalidSignature = 100,
        InvalidMerchant = 101,
        InvalidShopContract = 110,
        ForbiddenByShopAction = 120,
        BadRequestParameter = 200,
        TransactionNotFound = 340,
        TransactionIsBlocked = 350,
        TransactionExpired = 360,
        ReccurentProfileExpired = 365,
        PaymentCanceled = 400,
        PaymentCanceledLimitExceeded = 420,
        PaymentCannotBeCanceled = 490,
        GeneralError = 600,
        BadRequestByClient = 700,
        InvalidPhoneNumber = 701,
        InvalidPhoneNumberForPaymentSystem = 711,
        NoAvailablePaymentSystem = 850,
        InternalServerError = 1000
    }
}