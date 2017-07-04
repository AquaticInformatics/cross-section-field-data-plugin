using System;
using System.Globalization;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class DateTimeHelper
    {
        private const string Format = "yyyy-MM-dd HH:mm:ss";

        public static DateTime Parse(string value)
        {
            try
            {
                return DateTime.ParseExact(value, Format, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new PocketGaugerDataFormatException(string.Format("{0} is not a valid DateTime value", value));
            }
        }

        public static string Serialize(DateTime dateTime)
        {
            return dateTime.ToString(Format);
        }

        public static DateTime GetMean(DateTime start, DateTime end)
        {
            var meanTicks = (start.Ticks + end.Ticks) / 2;
            return new DateTime(meanTicks);
        }

        public static DateTimeOffset GetMean(DateTimeOffset start, DateTimeOffset end)
        {
            var meanTicks = (start.Ticks + end.Ticks) / 2;
            return new DateTimeOffset(meanTicks, start.Offset);
        }
    }
}
