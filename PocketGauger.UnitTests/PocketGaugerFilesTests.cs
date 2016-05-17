using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    public class PocketGaugerFilesTests
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Dispose_CallsDisposeOnEachItem()
        {
            var pocketGaugerFiles = new PocketGaugerFiles();
            var streams = new List<Stream>();
            for (var i = 0; i < 3; i++)
            {
                var stream = Substitute.For<Stream>();

                streams.Add(stream);
                pocketGaugerFiles.Add(_fixture.Create<string>(), stream);
            }

            pocketGaugerFiles.Dispose();

            streams.ForEach(s => s.Received().Dispose());
        }
    }
}
