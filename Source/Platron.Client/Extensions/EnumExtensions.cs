using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Platron.Client.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum enumeration)
        {
            return GetAttributeValue<DescriptionAttribute, string>(enumeration, x => x.Description, x => x.ToString());
        }

        public static Expected GetAttributeValue<T, Expected>(this Enum enumeration, Func<T, Expected> expression,
            Func<Enum, Expected> defaultValue)
            where T : Attribute
        {
            var attribute =
                enumeration
                    .GetType()
                    .GetMember(enumeration.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof (T), false)
                    .Cast<T>()
                    .SingleOrDefault();

            if (attribute == null)
            {
                return default(Expected);
            }

            return expression(attribute);
        }
    }
}