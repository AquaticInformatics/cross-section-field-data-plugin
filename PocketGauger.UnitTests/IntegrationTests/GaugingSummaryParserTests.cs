using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class GaugingSummaryParserTests : IntegrationTestBase
    {
        private GaugingSummaryParser _gaugingSummaryParser;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _gaugingSummaryParser = new GaugingSummaryParser();
        }

        [Test]
        public void Parse_PocketGaugerFilesContainsValidGaugingSummary_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.GaugingSummary);

            var result = _gaugingSummaryParser.Parse(PocketGaugerFiles);

            var expected = ExpectedGaugingSummaryData.CreateExpectedGaugingSummary();
            result.ShouldBeEquivalentTo(expected);
        }
    }
}
