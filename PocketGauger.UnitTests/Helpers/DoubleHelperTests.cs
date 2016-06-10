using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class DoubleHelperTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Parse_NullString_ReturnsNull()
        {
            var result = DoubleHelper.Parse(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Parse_NonDoubleString_ReturnsNull()
        {
            var result = DoubleHelper.Parse(_fixture.Create<string>());

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Parse_ValueIsPocketGuagerNullValue_ReturnsNull()
        {
            const string pocketGuagerNullValue = "-999999999.999";

            var result = DoubleHelper.Parse(pocketGuagerNullValue);

            Assert.That(result, Is.EqualTo(null));
        }

        private readonly List<Tuple<string, double>> _doubleValueTestCases = new List<Tuple<string, double>>
        {
            Tuple.Create("1234", 1234d),
            Tuple.Create("1234.4321", 1234.4321),
            Tuple.Create("0", 0d)
        };

        [TestCaseSource(nameof(_doubleValueTestCases))]
        public void Parse_ValidDoubleString_ReturnsExpectedValue(Tuple<string, double> testCases)
        {
            var value = testCases.Item1;
            var expected = testCases.Item2;

            var result = DoubleHelper.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(_doubleValueTestCases))]
        public void Serialize_DoubleHasValue_ReturnsExpectedString(Tuple<string, double> testCases)
        {
            var value = testCases.Item2;
            var expected = testCases.Item1;

            var result = DoubleHelper.Serialize(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Serialize_ValueIsNull_ReturnsEmptyString()
        {
            var result = DoubleHelper.Serialize(null);

            Assert.That(result, Is.Null);
        }
    }
}
