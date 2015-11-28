using System;

namespace Platron.Client.Tests
{
    public static class Samples
    {
        public static readonly Uri ResultUrlWithValidSignature =
            new Uri(
                "http://simplestore.com/platron/result?pg_salt=49c1e&pg_order_id=e76346a58c9c4c44bfa4e2f8600d9215&pg_payment_id=21404388&pg_amount=1.0000&pg_currency=RUR&pg_net_amount=0.94&pg_ps_amount=1&pg_ps_full_amount=1.00&pg_ps_currency=RUR&pg_payment_system=YANDEXMONEY&pg_result=1&pg_payment_date=2015-10-28+00%3A16%3A09&pg_can_reject=1&pg_user_phone=79261238736&pg_need_phone_notification=1&pg_sig=37d26729ec04e12b08e633e7530d5eb2");

        public static Credentials Credentials => new Credentials("0000", "secret");
    }
}