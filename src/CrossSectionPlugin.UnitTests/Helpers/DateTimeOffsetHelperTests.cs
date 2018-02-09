using System;
using System.Collections.Generic;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;
using NUnit.Framework;

namespace CrossSectionPlugin.UnitTests.Helpers
{
    [TestFixture]
    public class DateTimeOffsetHelperTests
    {
        [TestCaseSource(nameof(InvalidDateTimeStrings))]
        public void Parse_ValueIsNotValid_Throws(string unsupportedDateTimeOffset)
        {
            TestDelegate testDelegate = () => DateTimeOffsetHelper.Parse(unsupportedDateTimeOffset);

            Assert.That(testDelegate, Throws.TypeOf<CrossSectionSurveyDataFormatException>().With.Message.Contains("not in the expected DateTime format"));
        }

        private static readonly List<string> InvalidDateTimeStrings = new List<string>
        {
            "1974-01-01T00:00:00",
            "1980-05-02 00:00:00",
            "1996-02-29T15:55:14-7",
            "2000-01-01T00:30:15+03:80",
            "2016-05-24T09:49:23+3:5",
            Guid.NewGuid().ToString(),
            "",
            null
        };

        [TestCaseSource(nameof(ValidDateTimeStringsAndExpectedDateTimeOffsets))]
        public void Parse_ValueIsValidDateTime_ReturnsParsedValue(Tuple<string, DateTimeOffset> testPair)
        {
            var testValue = testPair.Item1;

            var result = DateTimeOffsetHelper.Parse(testValue);

            var expectedResult = testPair.Item2;
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private static List<Tuple<string, DateTimeOffset>> ValidDateTimeStringsAndExpectedDateTimeOffsets()
        {
            return new List<Tuple<string, DateTimeOffset>>
            {
                Tuple.Create("1970-01-01T00:00:00+07:00", new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.FromHours(7))),
                Tuple.Create("1996-02-29T15:55:14-07:30", new DateTimeOffset(1996, 2, 29, 15, 55, 14, TimeSpan.FromHours(-7.5))),
                Tuple.Create("2000-01-01T00:30:15+01:45", new DateTimeOffset(2000, 1, 1, 0, 30, 15, TimeSpan.FromHours(1.75))),
                Tuple.Create("2016-05-24T09:49:23+00:00", new DateTimeOffset(2016, 5, 24, 9, 49, 23, TimeSpan.FromHours(0))),
                Tuple.Create("2016-05-24T09:49:23Z", new DateTimeOffset(2016, 5, 24, 9, 49, 23, TimeSpan.FromHours(0)))
            };
        }
    }
}
