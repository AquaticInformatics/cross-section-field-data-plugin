using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class MeterDetailsParserTests : IntegrationTestBase
    {
        [Test]
        public void Parse_PocketGaugerFilesContainsMeterFiles_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.MeterDetails);
            AddPocketGaugerFile(FileNames.MeterCalibrations);

            var expected = ExpectedMeterDetailsData.CreateExpectedThreeMeterDetails();

            var result = MeterDetailsParser.Parse(PocketGaugerFiles);

            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
