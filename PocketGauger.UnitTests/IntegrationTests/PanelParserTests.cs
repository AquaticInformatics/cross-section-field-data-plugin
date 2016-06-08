using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class PanelParserTests : IntegrationTestBase
    {
        private PanelParser _parser;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _parser = new PanelParser();
        }

        [Test]
        public void Parse_PocketGaugerFilesContainsPanelFiles_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.Panels);
            AddPocketGaugerFile(FileNames.Verticals);

            var result = _parser.Parse(PocketGaugerFiles);

            var expected = ExpectedPanelData.CreateExpectedPanels();
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
