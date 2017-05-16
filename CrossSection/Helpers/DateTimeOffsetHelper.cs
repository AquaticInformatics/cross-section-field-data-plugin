using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPluginCore.Exceptions;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public class DateTimeOffsetHelper
    {
        private const string CrossSectionDateFormat = "yyyy-MM-ddTHH:mm:ss";
        private static readonly string[] UtcOffsets = { "Z", "zzz" };

        private static readonly string[] SupportedDateFormats =
            CreateDateTimeFormatsWithUtcOffsets();

        private static string[] CreateDateTimeFormatsWithUtcOffsets()
        {
            var formats = new List<string>();
            formats.AddRange(UtcOffsets.Select(utcOffset => CrossSectionDateFormat + utcOffset));

            return formats.ToArray();
        }

        public static DateTimeOffset Parse(string date)
        {
            DateTimeOffset result;

            if (DateTimeOffset.TryParseExact(date, SupportedDateFormats , CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out result))
            {
                return result;
            }

            var dateFormats = string.Join(" or ", SupportedDateFormats);

            throw new ParsingFailedException(Invariant($"{date} is not in the expected DateTime format: {dateFormats}"));
        }
    }
}
