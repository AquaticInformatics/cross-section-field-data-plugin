using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;
using Server.TestHelpers.IntegrationTests.Common.FixtureHelpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    class CsvDataParserTests
    {
        private CsvDataParser<DummyImportRecord> _csvDataParser;
        private IFixture _fixture;


        [SetUp]
        public void BeforeTest()
        {
            _csvDataParser = new CsvDataParser<DummyImportRecord>();
            _fixture = AutoFixtureHelper.GetCommonFixture();
        }

        private InMemoryCsvFile<DummyImportRecord> CreateDummyImportFile(DummyImportRecordBuilder recordBuilder, int rowCount = 5)
        {
            var importFile = new InMemoryCsvFile<DummyImportRecord>();
            for (var i = 0; i < rowCount; i++)
            {
                importFile.AddRecord(recordBuilder.AParametricRecord(i));
            }
            return importFile;
        }

        [Test]
        public void ParseCsvData_nullStream()
        {
            Action act = () => _csvDataParser.ParseCsvData(null);
            act.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ParseCsvData_emptyStream_returnsEmptyResults()
        { 
            Stream stream = new MemoryStream();
            var results = _csvDataParser.ParseCsvData(stream);
            results.Should().BeEmpty();
        }

        [Test]
        public void ParseCSVData_invalidDataInStream_returnsEmptyResults()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("This is not valid data for csv processing so don't get your hopes up"));
            IEnumerable<DummyImportRecord> results = _csvDataParser.ParseCsvData(stream);
            results.Should().BeEmpty();
        }

        [Test]
        public void ParseCSVData_invalidCsvDataInStream_returnsEmptyResults()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("Nope,Not,Today\nthis,won't,work"));
            IEnumerable<DummyImportRecord> results = _csvDataParser.ParseCsvData(stream);
            results.Should().BeEmpty();
        }

        [Test]
        public void ParseFile_validStream()
        {
            int rows = 5;
            var memFile = CreateDummyImportFile(DummyImportRecordBuilder.StartBuilding(), rows);
            var stream = memFile.GetInMemoryCsvFileStream();
            var results = _csvDataParser.ParseCsvData(stream);
            foreach (var dummyImportRecord in results)
            {
                rows--;
                ValidateRowData(dummyImportRecord);
            }
            rows.Should().Be(0);
        }

        private void ValidateRowData(DummyImportRecord record)
        {
            var rowId = record.RecordOrdinal;
            record.Id.Should().Contain(rowId.ToString());
            record.RecordString.Should().Contain(rowId.ToString());
            record.RecordDateTime.Should().Be(new DateTime());
            record.Validate();
        }
    }
}
