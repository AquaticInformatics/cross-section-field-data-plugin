using System;
using FileHelpers;
using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    class CsvDateTimeOffsetConverterTests
    {
        private CsvDateTimeOffsetConverter DateTimeConverter;
        [TestFixtureSetUp]
        public void Setup()
        {
            DateTimeConverter = new CsvDateTimeOffsetConverter();
        }

        [TestCase("2017-08-30T13:00:00.0000000Z")]
        [TestCase("2017-08-30T13:00:00.0000000-2:00")]
        [TestCase("2017-08-30T13:00:00.0000000+11:00")]
        [TestCase("2017-08-30T13:00:00Z")]
        [TestCase("2017-08-30T13:00:00-7:00")]
        [TestCase("2017-08-30T13:00:00+4:00")]
        public void ValidFormats_CanBeParsed_WithoutException(string dateString)
        {
            Action action = () => DateTimeConverter.StringToField(dateString);
            action.ShouldNotThrow();
        }

        [TestCase("2017 -08-30T13:00:00.0000000")]
        [TestCase("2017-08-30T13:00:00")]
        [TestCase("2017-08-30T13:00:00.0000000")]
        [TestCase("2017-08-30T13:00:00+45:00")]
        [TestCase("2017-08-30T13:00:00-8")]
        [TestCase("2017-08-30T13:00:00-18:00")]
        [TestCase("2017-08-40T13:00:00-7:00")]
        [TestCase("2017-00-10T13:00:00-7:00")]
        public void InvalidFormats_CannotBeParsed_WithoutThrowingException(string dateString)
        {
            Action action = () => DateTimeConverter.StringToField(dateString);
            action.ShouldThrow<ConvertException>();
        }
    }
}
