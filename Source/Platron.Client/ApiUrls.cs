using System;

namespace Platron.Client
{
    /// <summary>
    ///     Class for retrieving Platron ApI URLs.
    /// </summary>
    public static class ApiUrls
    {
        private static readonly Uri _initPaymentThroughUserBrowser = new Uri("payment.php", UriKind.Relative);
        private static readonly Uri _initPaymentDirectTransitionFromShop = new Uri("init_payment.php", UriKind.Relative);

        public static Uri InitPayment(InitPaymentResponseType urlType)
        {
            switch (urlType)
            {
                case InitPaymentResponseType.RedirectLink:
                    return _initPaymentDirectTransitionFromShop;
                case InitPaymentResponseType.HtmlForm:
                    return _initPaymentThroughUserBrowser;
                default:
                    throw new ArgumentOutOfRangeException(nameof(urlType), urlType, null);
            }
        }
    }

    public enum InitPaymentResponseType
    {
        RedirectLink,
        HtmlForm
    }
}