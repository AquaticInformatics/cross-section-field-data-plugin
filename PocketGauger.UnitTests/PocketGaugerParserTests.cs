using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.TestHelpers.NUnitExtensions;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using DataModel = Server.BusinessInterfaces.FieldDataPluginCore.DataModel;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    [LongRunning]
    public class PocketGaugerParserTests
    {
        private PocketGaugerParser _pocketGaugerParser;

        private Stream _stream;
        private IFieldDataResultsAppender _fieldDataResultsAppender;
        private ILog _logger;

        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _pocketGaugerParser = new PocketGaugerParser();
            _logger = null;
            _fieldDataResultsAppender = Substitute.For<IFieldDataResultsAppender>();

            const string testPath = @"Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData.PGData.zip";
            _stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testPath);
        }

        [TearDown]
        public void TearDown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        [Test]
        public void ParseFile_FileStreamIsNotAValidZipFile_ReturnsCannotParse()
        {
            _stream = new MemoryStream(_fixture.Create<byte[]>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, _fieldDataResultsAppender, _logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.CannotParse);
        }

        [Test]
        public void ParseFile_FileStreamZipDoesNotContainGaugingSummary_ReturnsCannotParse()
        {
            _stream = CreateZipStream(_fixture.Create<string>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, _fieldDataResultsAppender, _logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.CannotParse);
        }

        private Stream CreateZipStream(string zipEntryName)
        {
            var memoryStream = new MemoryStream();
            var zipOutputStream = new ZipOutputStream(memoryStream);
            AddZipEntry(zipOutputStream, zipEntryName);

            zipOutputStream.IsStreamOwner = false;
            zipOutputStream.Close();
            memoryStream.Position = 0;

            return memoryStream;
        }

        private void AddZipEntry(ZipOutputStream zipOutputStream, string zipEntryName)
        {
            var zipEntry = new ZipEntry(zipEntryName);
            zipOutputStream.PutNextEntry(zipEntry);

            const int entrySize = 4096;
            var sourceData = _fixture.CreateMany<byte>(entrySize).ToArray();
            var buffer = new byte[4096];
            StreamUtils.Copy(new MemoryStream(sourceData), zipOutputStream, buffer);
            zipOutputStream.CloseEntry();
        }

        [Test]
        public void ParseFile_ValidFileStreamZip_ReturnsExpectedNumberOfParsedResults()
        {
            const int expectedNumberOfDischargeActivities = 3;

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, _fieldDataResultsAppender, _logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.ParsedSuccessfully);

            _fieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddFieldVisit(Arg.Any<ILocation>(), Arg.Any<DataModel.FieldVisitDetails>());

            _fieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddDischargeActivity(Arg.Any<IFieldVisit>(), Arg.Any<DischargeActivity>());
        }

        [Test]
        public void ProcessGaugingSummary_ValidGaugingSummary_MapsObserverToParty()
        {
            var expectedNumberOfItems = 3;

            var startDate = _fixture.Create<DateTime>();
            var duration = _fixture.Create<TimeSpan>().Duration();
            var observer = _fixture.Create<string>();

            var gaugingSummaryItems = _fixture.Build<GaugingSummaryItem>()
                .OmitAutoProperties()
                .With(x => x.StartDate, startDate)
                .With(x => x.EndDate, startDate.Add(duration))
                .With(x => x.ObserversName, observer)
                .With(x => x.GaugingId)
                .With(x => x.Flow)
                .With(x => x.Area)
                .With(x => x.MeanVelocity)
                .With(x => x.PanelItems, new PanelItem[]{})
                .CreateMany(expectedNumberOfItems).ToList();

            var gaugingSummary = new GaugingSummary { GaugingSummaryItems = gaugingSummaryItems };

            _pocketGaugerParser.ProcessGaugingSummary(gaugingSummary, _fieldDataResultsAppender);

            _fieldDataResultsAppender
                .Received(expectedNumberOfItems)
                .AddFieldVisit(Arg.Any<ILocation>(), Arg.Is<DataModel.FieldVisitDetails>(x => x.Party == observer));

            _fieldDataResultsAppender
                .Received(expectedNumberOfItems)
                .AddDischargeActivity(Arg.Any<IFieldVisit>(), Arg.Is<DischargeActivity>(x => x.Party == observer));
        }
    }
}
