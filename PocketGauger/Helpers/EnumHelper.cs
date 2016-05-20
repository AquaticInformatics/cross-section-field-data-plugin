using System;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class EnumHelper
    {
        public static TEnum Map<TEnum>(int value) where TEnum : struct
        {
            if (!typeof (TEnum).IsEnum)
            {
                throw new ArgumentException(Invariant($"{typeof (TEnum).Name} is not an enum"));
            }

            return (TEnum) (object) value;
        }
    }
}
