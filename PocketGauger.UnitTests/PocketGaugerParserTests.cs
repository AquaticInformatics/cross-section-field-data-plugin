using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    public class PocketGaugerParserTests
    {
        private PocketGaugerParser _pocketGaugerParser;

        private IParseContext _parseContext;
        private ILog _logger;

        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _pocketGaugerParser = new PocketGaugerParser();
            _parseContext = null;
            _logger = null;
        }

        [Test]
        public void ParseFile_fileStreamIsNotAValidZipFile_Throws()
        {
            var streamThatIsNotAZipFile = new MemoryStream(_fixture.Create<byte[]>());

            TestDelegate testDelegate =
                () => _pocketGaugerParser.ParseFile(streamThatIsNotAZipFile, _parseContext, _logger);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>().With.Message.Contains("not a zip file"));
        }

        [Test]
        public void ParseFile_fileStreamZipDoesNotContainGaugingSummary_Throws()
        {
            var zipStream = CreateZipStream(_fixture.Create<string>());

            TestDelegate testDelegate = () => _pocketGaugerParser.ParseFile(zipStream, _parseContext, _logger);

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
        public void ParseFile_fileStreamZipContainsGaugingSummary_ReturnsEmptyParseResults()
        {
            var zipStream = CreateZipStream(FileNames.GaugingSummary);

            var result = _pocketGaugerParser.ParseFile(zipStream, _parseContext, _logger);

            Assert.That(result, Is.Empty);
        }
    }
}
