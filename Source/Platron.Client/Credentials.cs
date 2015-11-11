using Platron.Client.Utils;

namespace Platron.Client
{
    public sealed class Credentials
    {
        public Credentials(string merchantId, string secretKey)
        {
            Ensure.ArgumentNotNullOrEmptyString(merchantId, "merchantId");
            Ensure.ArgumentNotNullOrEmptyString(secretKey, "secretKey");

            MerchantId = merchantId;
            SecretKey = secretKey;
        }

        public string MerchantId { get; }
        public string SecretKey { get; }
    }
}