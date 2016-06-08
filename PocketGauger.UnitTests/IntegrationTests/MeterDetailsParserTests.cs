using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class MeterDetailsParserTests : IntegrationTestBase
    {
        private MeterDetailsParser _meterDetailsParser;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _meterDetailsParser = new MeterDetailsParser();
        }

        [Test]
        public void Parse_PocketGaugerFilesContainsMeterFiles_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.MeterDetails);
            AddPocketGaugerFile(FileNames.MeterCalibrations);

            var expected = ExpectedMeterDetailsData.CreateExpectedThreeMeterDetails();

            var result = _meterDetailsParser.Parse(PocketGaugerFiles);

            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
