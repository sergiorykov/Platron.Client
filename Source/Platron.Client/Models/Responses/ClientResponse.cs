using Platron.Client.Http.Plain;

namespace Platron.Client
{
    public abstract class ClientResponse<TPlainResponse>
        where TPlainResponse : PlainResponse, new()
    {
        public void Init(TPlainResponse plain)
        {
            InitCore(plain);
        }

        protected abstract void InitCore(TPlainResponse plain);
    }
}