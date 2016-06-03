using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class GaugingSummaryParserTests : IntegrationTestBase
    {
        [Test]
        public void Parse_PocketGaugerFilesContainsValidGaugingSummary_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.GaugingSummary);

            var result = GaugingSummaryParser.Parse(PocketGaugerFiles);

            var expected = ExpectedGaugingSummaryData.CreateExpectedGaugingSummary();
            result.ShouldBeEquivalentTo(expected);
        }
    }
}
