using System;
using System.Threading;

namespace Platron.Client.Authentication
{
    /// <summary>
    ///     Provides salt to sign requests.
    /// </summary>
    public static class SaltProvider
    {
        private static Func<string> generator = () => Guid.NewGuid().ToString("N");

        /// <summary>
        ///     Creates salt.
        /// </summary>
        /// <returns>Salt.</returns>
        public static string Generate()
        {
            var current = generator;
            return current();
        }

        /// <summary>
        ///     Overrides used salt generator.
        /// </summary>
        /// <param name="value">Salt generator.</param>
        public static void SetGenerator(Func<string> value)
        {
            Interlocked.Exchange(ref generator, value);
        }
    }
}