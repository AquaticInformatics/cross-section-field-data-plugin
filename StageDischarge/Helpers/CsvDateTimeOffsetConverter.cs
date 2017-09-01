using System;
using System.Globalization;
using FileHelpers;

namespace Server.Plugins.FieldVisit.StageDischarge.Helpers
{
    public class CsvDateTimeOffsetConverter : ConverterBase
    {
        public const string Format = "O"; // ISO 8601 Roundtrip

        public override object StringToField(string from)
        {
            DateTimeOffset dateTimeOffset;
            if (DateTimeOffset.TryParseExact(from, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset))
                return dateTimeOffset;

            throw new ConvertException(from, typeof(DateTimeOffset), $"Input string '{from}' must be in \"yyyy-MM-ddTHH:mm:ss.fffffffzzz\" format.\n\nhttps://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip");
        }

        public override string FieldToString(object from)
        {
            if (from == null) return "";

            var dateTimeOffset = (DateTimeOffset)from;
            return dateTimeOffset.ToString(Format, CultureInfo.InvariantCulture);
        }
    }
}
