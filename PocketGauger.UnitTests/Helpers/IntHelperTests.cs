using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class IntHelperTests
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
            var result = IntHelper.Parse(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Parse_NonIntString_ReturnsNull()
        {
            var result = IntHelper.Parse(_fixture.Create<string>());

            Assert.That(result, Is.Null);
        }

        private readonly List<Tuple<string, int>> _intValueTestCases = new List<Tuple<string, int>>
        {
            Tuple.Create("1234", 1234),
            Tuple.Create("-135", -135),
            Tuple.Create("0", 0)
        };

        [TestCaseSource(nameof(_intValueTestCases))]
        public void Parse_ValidIntString_ReturnsExpectedValue(Tuple<string, int> testCases)
        {
            var value = testCases.Item1;
            var expected = testCases.Item2;

            var result = IntHelper.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(_intValueTestCases))]
        public void Serialize_IntHasValue_ReturnsExpectedString(Tuple<string, int> testCases)
        {
            var value = testCases.Item2;
            var expected = testCases.Item1;

            var result = IntHelper.Serialize(value);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Serialize_ValueIsNull_ReturnsEmptyString()
        {
            var result = IntHelper.Serialize(null);

            Assert.That(result, Is.Null);
        }
    }
}
