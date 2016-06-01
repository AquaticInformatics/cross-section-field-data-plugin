using System.IO;
using System.Linq;
using System.Reflection;
using Common.TestHelpers.NUnitExtensions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    [LongRunning]
    public class PocketGaugerParserTests
    {
        private PocketGaugerParser _pocketGaugerParser;

        private Stream _stream;
        private IParseContext _parseContext;
        private ILog _logger;

        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _pocketGaugerParser = new PocketGaugerParser();
            _logger = null;
            _parseContext = new ParseContextTestHelper().CreateMockParseContext();

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
        public void ParseFile_FileStreamIsNotAValidZipFile_Throws()
        {
            _stream = new MemoryStream(_fixture.Create<byte[]>());

            TestDelegate testDelegate =
                () => _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>().With.Message.Contains("not a zip file"));
        }

        [Test]
        public void ParseFile_FileStreamZipDoesNotContainGaugingSummary_Throws()
        {
            _stream = CreateZipStream(_fixture.Create<string>());

            TestDelegate testDelegate = () => _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>()
                    .With.Message.Contains(Invariant($"does not contain file {FileNames.GaugingSummary}")));
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
        public void ParseFile_ReturnsResultsWithDischargeValues()
        {
            const int expectedNumberOfParsedResults = 3;

            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(results, Has.Count.EqualTo(expectedNumberOfParsedResults));
        }
    }
}
