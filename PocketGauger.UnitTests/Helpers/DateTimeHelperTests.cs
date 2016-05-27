using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [TestCaseSource(nameof(GetTestData))]
        public void Parse_ValueIsValidDateTime_ReturnsParsedValue(Tuple<string, DateTime> testPair )
        {
            var testValue = testPair.Item1;

            var result = DateTimeHelper.Parse(testValue);

            var expectedResult = testPair.Item2;
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static List<Tuple<string, DateTime>> GetTestData()
        {
            return new List<Tuple<string, DateTime>>
            {
                new Tuple<string, DateTime>("1970-01-01 00:00:00", new DateTime(1970, 1, 1, 0, 0, 0)),
                new Tuple<string, DateTime>("1996-02-29 15:55:14", new DateTime(1996, 2, 29, 15, 55, 14)),
                new Tuple<string, DateTime>("2016-05-24 09:49:23", new DateTime(2016, 5, 24, 9, 49, 23))
            };
        }

        [TestCaseSource(nameof(GetTestData))]
        public void Parse_ValueIsValidDateTime_ResultIsLocalDateTime(Tuple<string, DateTime> testPair)
        {
            var testValue = testPair.Item1;

            var result = DateTimeHelper.Parse(testValue);

            Assert.That(result.Kind, Is.EqualTo(DateTimeKind.Unspecified));
        }

        [Test]
        public void Parse_ValueIsNotValid_Throws()
        {
            TestDelegate testDelegate = () => DateTimeHelper.Parse(_fixture.Create<string>());

            Assert.That(testDelegate, Throws.TypeOf<ParsingFailedException>());
        }

        [TestCaseSource(nameof(GetTestData))]
        public void Serialize_ReturnsCorrectString(Tuple<string, DateTime> testPair)
        {
            var testValue = testPair.Item2;

            var result = DateTimeHelper.Serialize(testValue);

            var expectedResult = testPair.Item1;
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
