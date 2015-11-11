using Platron.Client.Extensions;
using Platron.Client.Http.Plain;

namespace Platron.Client
{
    public abstract class ClientRequest
    {
        private bool _testMode;

        public void InTestMode()
        {
            _testMode = true;
        }

        public PlainRequest GetPlain()
        {
            var plain = GetPlainCore();
            if (_testMode)
            {
                plain.TestingMode = _testMode.ToZeroOrOne();
            }

            return plain;
        }

        protected abstract PlainRequest GetPlainCore();
    }
}