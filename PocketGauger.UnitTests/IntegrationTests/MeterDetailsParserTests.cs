using FluentAssertions;
using NUnit.Framework;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class MeterDetailsParserTests : IntegrationTestBase
    {
        [Test]
        public void Parse_PocketGaugerFilesContainsMeterFiles_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.MeterDetails);
            AddPocketGaugerFile(FileNames.MeterCalibrations);

            var expected = ExpectedTestData.CreateExpectedThreeMeterDetails();

            var result = MeterDetailsParser.Parse(PocketGaugerFiles);

            result.ShouldAllBeEquivalentTo(expected);
        }

        [Test]
        public void Parse_PocketGaugerFilesIsMissingMeterCalibrationFile_ThrowsParsingFailedException()
        {
            AddPocketGaugerFile(FileNames.MeterDetails);

            TestDelegate testDelegate = () => MeterDetailsParser.Parse(PocketGaugerFiles);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>());
        }

        [Test]
        public void Parse_PocketGaugerFilesIsMissingMeterDetailsFile_ThrowsParsingFailedException()
        {
            AddPocketGaugerFile(FileNames.MeterCalibrations);

            TestDelegate testDelegate = () => MeterDetailsParser.Parse(PocketGaugerFiles);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>());
        }
    }
}
