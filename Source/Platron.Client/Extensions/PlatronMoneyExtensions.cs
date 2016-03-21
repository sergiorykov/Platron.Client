namespace Platron.Client.Extensions
{
    public static class PlatronMoneyExtensions
    {
        public static PlatronMoney Rub(this double value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Rub);
        }

        public static PlatronMoney Rub(this int value)
        {
            return new PlatronMoney(value, PlatronPaymentCurrency.Rub);
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