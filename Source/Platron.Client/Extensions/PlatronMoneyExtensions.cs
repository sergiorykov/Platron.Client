namespace Platron.Client.Extensions
{
    public static class PlatronMoneyExtensions
    {
        public static PlatronMoney Rur(this double value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Rur);
        }

        public static PlatronMoney Rur(this int value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Rur);
        }

        public static PlatronMoney Eur(this double value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Eur);
        }

        public static PlatronMoney Eur(this int value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Eur);
        }

        public static PlatronMoney Usd(this double value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Usd);
        }

        public static PlatronMoney Usd(this int value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Usd);
        }
    }
}