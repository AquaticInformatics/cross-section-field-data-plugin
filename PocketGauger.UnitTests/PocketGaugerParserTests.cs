using System.IO;
using System.Linq;
using System.Reflection;
using Common.TestHelpers.NUnitExtensions;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
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
        public void ParseFile_FileStreamIsNotAValidZipFile_ReturnsFileFormatNotSupported()
        {
            _stream = new MemoryStream(_fixture.Create<byte[]>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, _fieldDataResultsAppender, _logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.FileFormatNotSupported);
        }

        [Test]
        public void ParseFile_FileStreamZipDoesNotContainGaugingSummary_ReturnsFileFormatNotSupported()
        {
            _stream = CreateZipStream(_fixture.Create<string>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, _fieldDataResultsAppender, _logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.FileFormatNotSupported);
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
            parseFileResult.Status.Should().Be(ParseFileStatus.Success);

            _fieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddFieldVisit(Arg.Any<ILocation>(), Arg.Any<DataModel.FieldVisitDetails>());

            _fieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddDischargeActivity(Arg.Any<IFieldVisit>(), Arg.Any<DischargeActivity>());
        }
    }
}
