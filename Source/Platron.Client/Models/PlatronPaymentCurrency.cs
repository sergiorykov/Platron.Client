using System.ComponentModel;

namespace Platron.Client
{
    public enum PlatronPaymentCurrency
    {
        None,

        [Description("RUB")]
        Rub,

        [Description("USD")]
        Usd,

        [Description("EUR")]
        Eur
    }
}