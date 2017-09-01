using System;
using System.IO;
using System.Text;
using FileHelpers;
using FluentAssertions;
using NUnit.Framework;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    internal class CsvDataParserTests
    {
        private CsvDataParser<DummyImportRecord> _csvDataParser;

        [SetUp]
        public void BeforeTest()
        {
            _csvDataParser = new CsvDataParser<DummyImportRecord>();
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
        public void ParseCsvData_WithNullStream_ThrowsException()
        {
            Action parseAction = () => _csvDataParser.ParseInputData(null);
            parseAction.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void ParseCsvData_WithEmptyStream_ReturnsEmptyResults()
        {
            using (Stream stream = new MemoryStream())
            { 
                var results = _csvDataParser.ParseInputData(stream);
                results.Should().BeEmpty();
            }
        }

        [Test]
        public void ParseCsvData_InvalidDataInStream_ReturnsEmptyResults()
        {
            using (var stream =
                new MemoryStream(
                    Encoding.ASCII.GetBytes("This is not valid data for csv processing so don't get your hopes up")))
            {
                Action parseAction = () => _csvDataParser.ParseInputData(stream);
                parseAction.ShouldThrow<FileHelpersException>();
            }
        }

        [Test]
        public void ParseCsvData_InvalidCsvDataInStream_ReturnsEmptyResults()
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes("Nope,Not,Today\nthis,won't,work")))
            {
                Action parseAction = () => _csvDataParser.ParseInputData(stream);
                parseAction.ShouldThrow<FileHelpersException>();
            }
        }

        [Test]
        public void ParseFile_ValidStream_ReadsAllRows()
        {
            var memFile = CreateDummyImportFile(DummyImportRecordBuilder.StartBuilding());
            {
                using (var stream = memFile.GetInMemoryCsvFileStream())
                {
                    var results = _csvDataParser.ParseInputData(stream);
                    foreach (var dummyImportRecord in results)
                    {
                        ValidateRowData(dummyImportRecord);
                    }
                }
            }
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
