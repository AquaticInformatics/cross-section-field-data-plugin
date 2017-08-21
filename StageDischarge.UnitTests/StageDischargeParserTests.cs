using Common.TestHelpers.NUnitExtensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    [LongRunning]
    public class StageDischargeParserTests
    {
        private Fixture _fixture;
        private IFieldDataResultsAppender _mockAppender;
        private ILog _mockLogger;
        private StageDischargeParser _csvDataParser;

        [SetUp]
        public void BeforeTest()
        {
            _fixture = new Fixture();
            _mockAppender = Substitute.For<IFieldDataResultsAppender>();
            _mockLogger = Substitute.For<ILog>();
            _csvDataParser = new StageDischargeParser();
        }

        [Test]
        public void ParseFile_WithValidCsvInputFile_ReadsStageDischargeRecords()
        {
            var csvFile = new InMemoryCsvFile<StageDischargeRecord>();
            StageDischargeRecord originalRecord = HappyPathStageDischargeCsvFileBuilder.CreateFullRecord();
            csvFile.AddRecord(originalRecord);
            var stageDischargeParser = Substitute.For<StageDischargeParser>();
            var stream = csvFile.GetInMemoryCsvFileStream();
            var results = _csvDataParser.ParseFile(stream, _mockAppender, _mockLogger);
            results.Status.Should().NotBe(ParseFileStatus.CannotParse, results.ErrorMessage);

        }
    }
}
