using System;
using System.Globalization;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class DateTimeHelper
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public static DateTime Parse(string value)
        {
            return DateTime.ParseExact(value, Format, CultureInfo.InvariantCulture);
        }

        public static string Serialize(DateTime dateTime)
        {
            return dateTime.ToString(Format);
        }
    }
}
