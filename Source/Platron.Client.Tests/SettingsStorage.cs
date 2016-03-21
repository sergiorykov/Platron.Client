using System;

namespace Platron.Client.Tests
{
    public static class SettingsStorage
    {
        public static Credentials Credentials => new Credentials(
            GetFromEnvironment("PLATRON_TEST_MERCHANTID"),
            GetFromEnvironment("PLATRON_TEST_SECRETKEY"));

        public static string PhoneNumber => GetFromEnvironment("PLATRON_TEST_PHONENUMBER");

        private static string GetFromEnvironment(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Set up environment variable: {name}");
            }

            return value;
        }
    }
}
