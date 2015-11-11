using System.ComponentModel;

namespace Platron.Client
{
    public enum PlatronPaymentCurrency
    {
        None,

        [Description("RUR")] Rur,

        [Description("USD")] Usd,

        [Description("EUR")] Eur
    }
}