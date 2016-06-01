using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class PanelParserTests : IntegrationTestBase
    {
        [Test]
        public void Parse_PocketGaugerFilesContainsPanelFiles_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.Panels);
            AddPocketGaugerFile(FileNames.Verticals);

            var result = PanelParser.Parse(PocketGaugerFiles);

            var expected = ExpectedPanelData.CreateExpectedPanels();
            result.ShouldAllBeEquivalentTo(expected);
        }
    }
}
