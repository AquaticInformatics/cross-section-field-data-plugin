using System;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public static class ExtensionMethods
    {
        public static bool EqualsOrdinalIgnoreCase(this string value, string otherValue)
        {
            return string.Equals(value, otherValue, StringComparison.OrdinalIgnoreCase);
        }

        public static double? ToNullableDouble(this string value)
        {
            return DoubleHelper.Parse(value);
        }

        public static DateTimeOffset ToDateTimeOffset(this string value)
        {
            return DateTimeOffsetHelper.Parse(value);
        }

        public static StartPointType ToStartPointType(this string value)
        {
            return StartPointTypeHelper.Parse(value);
        }
    }
}
