using System.IO;
using System.Linq;
using System.Reflection;
using Common.TestHelpers.NUnitExtensions;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.Results;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    public class PocketGaugerParserTests : PocketGaugerTestsBase
    {
        private PocketGaugerParser _pocketGaugerParser;
        private Stream _stream;

        [SetUp]
        public new void SetUp()
        {
            _pocketGaugerParser = new PocketGaugerParser();

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
            _stream = new MemoryStream(Fixture.Create<byte[]>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, FieldDataResultsAppender, Logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.CannotParse);
        }

        [Test]
        public void ParseFile_FileStreamZipDoesNotContainGaugingSummary_ReturnsCannotParse()
        {
            _stream = CreateZipStream(Fixture.Create<string>());

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, FieldDataResultsAppender, Logger);
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
            var sourceData = Fixture.CreateMany<byte>(entrySize).ToArray();
            var buffer = new byte[4096];
            StreamUtils.Copy(new MemoryStream(sourceData), zipOutputStream, buffer);
            zipOutputStream.CloseEntry();
        }

        [Test]
        public void ParseFile_ValidFileStreamZip_ReturnsExpectedNumberOfParsedResults()
        {
            const int expectedNumberOfDischargeActivities = 3;

            var parseFileResult = _pocketGaugerParser.ParseFile(_stream, FieldDataResultsAppender, Logger);
            parseFileResult.Status.Should().Be(ParseFileStatus.SuccessfullyParsedAndDataValid);

            FieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Any<FieldVisitDetails>());

            FieldDataResultsAppender
                .Received(expectedNumberOfDischargeActivities)
                .AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Any<DischargeActivity>());
        }
    }
}
