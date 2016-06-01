using System;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class EnumHelper
    {
        public static TEnum? Map<TEnum>(string value) where TEnum : struct
        {
            if (!typeof (TEnum).IsEnum)
            {
                throw new ArgumentException(Invariant($"{typeof (TEnum).Name} is not an enum"));
            }
            if (value == null) return null;

            TEnum enumValue;
            if (Enum.TryParse(value, out enumValue) && Enum.IsDefined(typeof(TEnum), enumValue))
                return enumValue;

            return default(TEnum?);
        }
    }
}
