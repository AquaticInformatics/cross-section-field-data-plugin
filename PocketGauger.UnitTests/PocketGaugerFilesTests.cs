using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    public class PocketGaugerFilesTests
    {
        private IFixture _fixture;
        private PocketGaugerFiles _pocketGaugerFiles;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _pocketGaugerFiles = new PocketGaugerFiles();
        }

        [TearDown]
        public void TearDown()
        {
            _pocketGaugerFiles?.Dispose();
        }

        [Test]
        public void Dispose_CallsDisposeOnEachItem()
        {
            var streams = new List<Stream>();
            for (var i = 0; i < 3; i++)
            {
                var stream = Substitute.For<Stream>();

                streams.Add(stream);
                _pocketGaugerFiles.Add(_fixture.Create<string>(), stream);
            }

            _pocketGaugerFiles.Dispose();

            streams.ForEach(s => s.Received().Dispose());
        }

        [Test]
        public void ParseType_TypeNotInFileNameTypeMap_ThrowsArgumentException()
        {
            TestDelegate testDelegate = () => _pocketGaugerFiles.ParseType<string>();

            Assert.That(testDelegate, Throws.ArgumentException);
        }

        [Test]
        public void ParseType_TypeInFileNameTypeMapButFileIsNotPresent_ThrowsPocketGaugerZipFileMissingRequiredContentException()
        {
            TestDelegate testDelegate = () => _pocketGaugerFiles.ParseType<GaugingSummary>();

            Assert.That(testDelegate, Throws.Exception.TypeOf<PocketGaugerZipFileMissingRequiredContentException>());
        }
    }
}
