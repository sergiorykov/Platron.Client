namespace Platron.Client
{
    public sealed class PlatronMoney
    {
        public PlatronMoney(double amount, PlatronPaymentCurrency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public double Amount { get; }
        public PlatronPaymentCurrency Currency { get; }
    }
}