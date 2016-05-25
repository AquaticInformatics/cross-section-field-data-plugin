using System;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class EnumHelperTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Map_ReturnsValueAsEnum()
        {
            const int testValue = 1;

            var result = EnumHelper.Map<BankSide>(testValue);

            Assert.That(result, Is.EqualTo(BankSide.Right));
        }

        [Test]
        public void Map_TEnumIsNotAnEnum_Throws()
        {
            TestDelegate testDelegate = () => EnumHelper.Map<DateTime>(_fixture.Create<int>());

            Assert.That(testDelegate, Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Map_ValueIsOutsideOfEnumRange_ReturnsNull()
        {
            var testValue = GetIntOutsideOfGaugingMethodRange();

            var result = EnumHelper.Map<GaugingMethod>(testValue);

            Assert.That(result, Is.Null);
        }

        private static int GetIntOutsideOfGaugingMethodRange()
        {
            var maximumValueForGaugingMethod =
                Enum.GetValues(typeof (GaugingMethod)).OfType<GaugingMethod>().Cast<int>().Max();

            return maximumValueForGaugingMethod + 1;
        }
    }
}
