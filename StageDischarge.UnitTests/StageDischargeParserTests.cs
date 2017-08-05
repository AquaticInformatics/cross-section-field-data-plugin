using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.TestHelpers.NUnitExtensions;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.StageDischarge.Interfaces;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData;
using Server.TestHelpers.FieldVisitTestHelpers.TestHelpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    [LongRunning]
    public class StageDischargeParserTests
    {
        private Fixture _fixture;
        private IFieldDataResultsAppender _mockAppender;
        private ILog _mockLogger;
        private StageDischargePlugin _csvDataPlugin;

        [SetUp]
        public void BeforeTest()
        {
            _fixture = new Fixture();
            _mockAppender = Substitute.For<IFieldDataResultsAppender>();
            _mockLogger = Substitute.For<ILog>();
            _csvDataPlugin = new StageDischargePlugin(new CsvDataParser<StageDischargeRecord>());
        }

        [Test]
        public void ParseFile_WithOneValidRowInCsvInputFile_ReadsAndSavesStageDischargeRecordsAndReturnsSuccess()
        {
            var stream = CreateValidCsvFileStream();
            _mockAppender.GetLocationByIdentifier(Arg.Any<string>()).Returns(LocationInfoHelper.GetTestLocationInfo(_fixture));

            var results = _csvDataPlugin.ParseFile(stream, _mockAppender, _mockLogger);

            results.Status.Should().NotBe(ParseFileStatus.CannotParse);
            results.Status.Should().Be(ParseFileStatus.SuccessfullyParsedAndDataValid);

            _mockAppender.Received().AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());
            _mockAppender.Received(1).AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());
        }
        
        [Test]
        public void ParseFile_WithSpecifiedLocation_callsLocationlessParseFile()
        {
            var stream = CreateValidCsvFileStream();
            var location = LocationInfoHelper.GetTestLocationInfo(_fixture);
            _mockAppender.GetLocationByIdentifier(Arg.Any<string>()).Returns(LocationInfoHelper.GetTestLocationInfo(_fixture));

            var results = _csvDataPlugin.ParseFile(stream, location, _mockAppender, _mockLogger);

            results.Status.Should().NotBe(ParseFileStatus.CannotParse);
            results.Status.Should().Be(ParseFileStatus.SuccessfullyParsedAndDataValid);

            _mockAppender.Received().AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());
            _mockAppender.Received(1).AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());

        }

        private Stream CreateValidCsvFileStream()
        {
            var csvFile = new InMemoryCsvFile<StageDischargeRecord>();
            StageDischargeRecord originalRecord = StageDischargeCsvFileBuilder.CreateFullRecord(_fixture);
            csvFile.AddRecord(originalRecord);
            return csvFile.GetInMemoryCsvFileStream();
        }

        [Test]
        public void ParseFile_WithFiveValidRowsInCsvInputFile_ReadsAndSavesAndReturnsSuccess()
        {
            var csvFile = new InMemoryCsvFile<StageDischargeRecord>();
            PutNRecordsInCsvFile(5, ref csvFile);
            var stream = csvFile.GetInMemoryCsvFileStream();
            _mockAppender.GetLocationByIdentifier(Arg.Any<string>())
                .Returns(LocationInfoHelper.GetTestLocationInfo(_fixture),
                    LocationInfoHelper.GetTestLocationInfo(_fixture),
                    LocationInfoHelper.GetTestLocationInfo(_fixture),
                    LocationInfoHelper.GetTestLocationInfo(_fixture),
                    LocationInfoHelper.GetTestLocationInfo(_fixture));

            var results = _csvDataPlugin.ParseFile(stream, _mockAppender, _mockLogger);

            results.Status.Should().NotBe(ParseFileStatus.CannotParse, results.ErrorMessage);
            results.Status.Should().Be(ParseFileStatus.SuccessfullyParsedAndDataValid);

            _mockAppender.Received().AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());
            _mockAppender.Received(5).AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());
        }

        private void PutNRecordsInCsvFile(int recordCount, ref InMemoryCsvFile<StageDischargeRecord> csvFile)
        {
            for (var i = 0; i < recordCount; i++)
            {
                StageDischargeRecord rRecord = _fixture.Build<StageDischargeRecord>()
                    .With(x => x.MeasurementStartDateTime, DateTime.Now.AddHours(i))
                    .With(x => x.MeasurementEndDateTime, DateTime.Now.AddHours(i * 2))
                    .Create();
                csvFile.AddRecord(rRecord);
            }

        }

        [Test]
        public void ParseFile_WithOneValidRowInCsvInputFileButUnknownLocation_SavesNothingAndReturnsCannotParse()
        {
            var stream = CreateValidCsvFileStream();
            _mockAppender.GetLocationByIdentifier(Arg.Any<string>()).Throws(new ArgumentException("nope nope nope"));
            var results = _csvDataPlugin.ParseFile(stream, _mockAppender, _mockLogger);
            results.Status.Should().Be(ParseFileStatus.CannotParse);
            results.ErrorMessage.Should().Contain("nope nope nope");
            _mockAppender.DidNotReceive().AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());
            _mockAppender.DidNotReceive().AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());
        }


        [Test]
        public void ParseFile_WithInvalidRowInputFile_SavesNothingAndReturnsCannotParse()
        {
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("Nope,Not,Today\nthis,won't,work"));
            var results = _csvDataPlugin.ParseFile(stream, _mockAppender, _mockLogger);

            results.Status.Should().Be(ParseFileStatus.CannotParse, results.ErrorMessage);
            _mockAppender.DidNotReceive().AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());
            _mockAppender.DidNotReceive().AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());
        }

        [Test]
        public void ParseFile_WithNoRecords_ReturnsError()
        {
            var mockCsvDataParser = Substitute.For<IDataParser<StageDischargeRecord>>();
            mockCsvDataParser.ParseInputData(Arg.Any<Stream>()).Returns((IEnumerable<StageDischargeRecord>)null);
            var plugin = new StageDischargePlugin(mockCsvDataParser);

            var stream = new MemoryStream(Encoding.ASCII.GetBytes("Nothing to see here folks..."));
            var results = plugin.ParseFile(stream, _mockAppender, _mockLogger);
            results.Status.Should().Be(ParseFileStatus.CannotParse);
            results.ErrorMessage.Should().Contain(StageDischargePlugin.NoRecordsInInputFile);
        }

        [Test]
        public void ParseFile_WithInvalidRecords_ReturnsError()
        {
            var mockCsvDataParser = Substitute.For<IDataParser<StageDischargeRecord>>();
            mockCsvDataParser.ParseInputData(Arg.Any<Stream>()).Returns(_fixture.Create<IEnumerable<StageDischargeRecord>>());
            mockCsvDataParser.InvalidRecords.Returns(5);
            var plugin = new StageDischargePlugin(mockCsvDataParser);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes("Good and bad data"));
            var results = plugin.ParseFile(stream, _mockAppender, _mockLogger);
            results.Status.Should().Be(ParseFileStatus.SuccessfullyParsedButDataInvalid);
            results.ErrorMessage.Should().Contain(StageDischargePlugin.InputFileContainsInvalidRecords);
        }
    }
}
