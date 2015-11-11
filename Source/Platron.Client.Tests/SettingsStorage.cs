using System;

namespace Platron.Client.Tests
{
    public static class SettingsStorage
    {
        public static Credentials Credentials => new Credentials(
            Environment.GetEnvironmentVariable("PLATRON_MERCHANTID"),
            Environment.GetEnvironmentVariable("PLATRON_SECRETKEY"));
    }
}