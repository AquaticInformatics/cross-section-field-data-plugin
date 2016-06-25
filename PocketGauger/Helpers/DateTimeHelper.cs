using System;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;

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
                throw new ParsingFailedException(string.Format("{0} is not a valid DateTime value", value));
            }
        }

        public static string Serialize(DateTime dateTime)
        {
            return dateTime.ToString(Format);
        }

        public static DateTimeOffset GetMeanTime(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var startTicks = startTime.Ticks;
            var endTicks = endTime.Ticks;

            var meanTicks = (startTicks + endTicks) / 2;

            return new DateTimeOffset(meanTicks, startTime.Offset);
        }
    }
}
