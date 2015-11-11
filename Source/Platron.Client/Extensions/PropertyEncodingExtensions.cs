using System;
using System.Globalization;

namespace Platron.Client.Extensions
{
    internal static class PropertyEncodingExtensions
    {
        public static string ToZeroOrOne(this bool value)
        {
            return value ? "1" : "0";
        }

        public static string ToZeroOrOne(this bool? value)
        {
            if (value.HasValue)
            {
                return ToZeroOrOne(value.Value);
            }

            return null;
        }

        public static string ToPlatronTime(this TimeSpan value)
        {
            return ((int) value.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }
    }
}