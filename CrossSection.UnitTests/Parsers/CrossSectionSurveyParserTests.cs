using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common.TestHelpers.NUnitExtensions;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Parsers
{
    [TestFixture, LongRunning]
    public class CrossSectionSurveyParserTests
    {
        private const string NamespacePrefix = "Server.Plugins.FieldVisit.CrossSection.UnitTests.TestData.";

        private const string ValidCrossSectionFilePath = NamespacePrefix + "CrossSection.csv";
        private const string DuplicateMetadataCrossSectionFilePath = NamespacePrefix + "CrossSectionWithDuplicateLocationMetadata.csv";

        private Stream _stream;

        private IFixture _fixture;
        private ICrossSectionParser _crossSectionParser;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _fixture = new Fixture();
            _crossSectionParser = new CrossSectionSurveyParser();
        }

        [TearDown]
        public void TearDown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        [Test]
        public void ParseFile_FileStreamIsNotAValidCrossSectionFile_Throws()
        {
            _stream = new MemoryStream(_fixture.Create<byte[]>());

            TestDelegate testDelegate =
                () => _crossSectionParser.ParseFile(_stream);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>().With.Message.Contains("not an AQUARIUS Cross-Section"));
        }

        private static readonly IEnumerable<string> ValidFileHeaders = new[]
        {
            CrossSectionParserConstants.Header,
            CrossSectionParserConstants.Header.ToLowerInvariant(),
            CrossSectionParserConstants.Header.ToUpperInvariant(),
            CrossSectionParserConstants.Header + "Some Other Text"
        };

        [TestCaseSource(nameof(ValidFileHeaders))]
        public void ParseFile_FileStreamIsAValidCrossSectionFile_DoesNotThrow(string header)
        {
            _stream = SetupMemoryStreamWithText(header);

            var parsedResults = _crossSectionParser.ParseFile(_stream);

            Assert.That(parsedResults, Is.Not.Null);
        }

        private static MemoryStream SetupMemoryStreamWithText(string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        }

        private static readonly IEnumerable<TestCaseData> VersionTests = new[]
        {
            new TestCaseData("v1.2.5"),
            new TestCaseData("55.4"),
            new TestCaseData("version2.24.5.4"),
            new TestCaseData("0.0.0")
        };

        [TestCaseSource(nameof(VersionTests))]
        public void ParseFile_ValidCrossSectionFile_ParsesFileVersionPropertyFromHeader(string versionString)
        {
            _stream = SetupMemoryStreamWithText(CrossSectionParserConstants.Header + versionString);

            var parsedResults = _crossSectionParser.ParseFile(_stream);

            versionString.Should().Contain(parsedResults.CsvFileVersion.ToString());
        }

        [Test]
        public void ParseFile_UnknownHeaderVersionString_SetsVersionToDefault()
        {
            var expectedVersion = new Version(CrossSectionParserConstants.DefaultVersion);

            _stream = SetupMemoryStreamWithText(CrossSectionParserConstants.Header + "versionTwoPointThree");

            var parsedResults = _crossSectionParser.ParseFile(_stream);

            parsedResults.CsvFileVersion.Should().Be(expectedVersion);
        }

        [Test]
        public void ParseFile_FileStreamIsAValidCrossSectionFile_StreamRemainsOpenAfterParsing()
        {
            _stream = GetTestFile(ValidCrossSectionFilePath);

            _crossSectionParser.ParseFile(_stream);

            Assert.That(_stream, Is.Not.Null, "Stream was disposed by plug-in");
        }

        private static Stream GetTestFile(string testPath)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(testPath);
        }

        [Test]
        public void ParseFile_FileHasDuplicateMetadataRecords_ThrowsException()
        {
            _stream = GetTestFile(DuplicateMetadataCrossSectionFilePath);

            TestDelegate testDelegate =
                () => _crossSectionParser.ParseFile(_stream);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<ParsingFailedException>().With.Message.Contains("File has duplicate Location records"));
        }

        [Test]
        public void ParseFile_FileStreamIsAValidCrossSectionFile_CreatesExpectedMetadata()
        {
            var expectedMetadata = CreateExpectedMetadata();

            _stream = GetTestFile(ValidCrossSectionFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            result.Metadata.ShouldBeEquivalentTo(expectedMetadata);
        }

        private static IDictionary<string, string> CreateExpectedMetadata()
        {
            return new Dictionary<string, string>
            {
                { "Location", "Server.Plugins.FieldVisit.CrossSection.Tests.CompleteCrossSection" },
                { "StartDate", "2001-05-08T14:32:15+07:00" },
                { "EndDate", "2001-05-08T17:12:45+07:00" },
                { "Party", "Cross-Section Party" },
                { "Channel", "Right overflow" },
                { "RelativeLocation", "At the Gage" },
                { "Stage", "12.2" },
                { "Unit", "ft" },
                { "StartBank", "Left bank" },
                { "Comment", "Cross-section survey comments" }
            };
        }

        [Test]
        public void ParseFile_FileStreamIsAValidCrossSectionFile_CreatesExpectedPointsCollection()
        {
            var expectedPoints = CreateExpectedCrossSectionPoints();

            _stream = GetTestFile(ValidCrossSectionFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            AssertPointsMatchesExpected(result.Points, expectedPoints);
        }

        private static List<CrossSectionPoint> CreateExpectedCrossSectionPoints()
        {
            return new List<CrossSectionPoint>
            {
                new CrossSectionPoint { Distance = 0, Elevation = 7.467, Comment = string.Empty },
                new CrossSectionPoint { Distance = 19.1, Elevation = 6.909, Comment = "some comment" },
                new CrossSectionPoint { Distance = 44.8, Elevation = 6.3, Comment = "yet, another, comment" },
                new CrossSectionPoint { Distance = 70.1, Elevation = 5.356, Comment = "another comment" },
                new CrossSectionPoint { Distance = 82.4, Elevation = 5.287, Comment = string.Empty }
            };
        }

        private static void AssertPointsMatchesExpected(List<CrossSectionPoint> actualPoints, List<CrossSectionPoint> expectedPoints)
        {
            foreach (var point in actualPoints.Zip(expectedPoints, Tuple.Create))
            {
                var actual = point.Item1;
                var expectation = point.Item2;

                AssertPointIsEqual(actual, expectation);
            }
        }

        private static void AssertPointIsEqual(CrossSectionPoint actual, CrossSectionPoint expectation)
        {
            Assert.That(actual.Distance, Is.EqualTo(expectation.Distance));
            Assert.That(actual.Elevation, Is.EqualTo(expectation.Elevation));
            Assert.That(actual.Comment, Is.EqualTo(expectation.Comment));
        }
    }
}
