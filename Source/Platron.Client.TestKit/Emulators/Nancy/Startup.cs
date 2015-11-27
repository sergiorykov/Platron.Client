using Owin;

namespace Platron.Client.TestKit.Emulators.Nancy
{
    public sealed class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}