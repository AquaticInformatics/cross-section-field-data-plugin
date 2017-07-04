using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class BooleanHelperTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [TestCase(BooleanHelper.True, true)]
        [TestCase(BooleanHelper.False, false)]
        public void Parse_ReturnsExpectedValue(string testValue, bool expectedResult)
        {
            var result = BooleanHelper.Parse(testValue);

            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Parse_ValueIsInvalid_Throws()
        {
            TestDelegate testDelegate = () => BooleanHelper.Parse(_fixture.Create<string>());

            Assert.That(testDelegate, Throws.TypeOf<PocketGaugerDataFormatException>());
        }

        [TestCase(true, BooleanHelper.True)]
        [TestCase(false, BooleanHelper.False)]
        public void Serialize_ReturnsCorrectString(bool testValue, string expectedResult)
        {
            var result = BooleanHelper.Serialize(testValue);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
