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
using Server.Plugins.FieldVisit.CrossSection.Exceptions;
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
        private const string DuplicateMetadataCrossSectionFilePath = NamespacePrefix + "CrossSectionWithDuplicateLocationFields.csv";
        private const string NegativeValuesFilePath = NamespacePrefix + "CrossSectionWithNegativeValues.csv";

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
                Throws.Exception.TypeOf<CrossSectionCsvFormatException>().With.Message.Contains("not an AQUARIUS Cross-Section"));
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
                Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>().With.Message.Contains("File has duplicate Location records"));
        }

        [Test]
        public void ParseFile_FileStreamIsAValidCrossSectionFile_CreatesExpectedMetadata()
        {
            var expectedMetadata = TestData.TestHelpers.CreateExpectedCrossSectionFields();

            _stream = GetTestFile(ValidCrossSectionFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            result.Fields.ShouldBeEquivalentTo(expectedMetadata);
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
                CreatePoint(0, 7.467),
                CreatePoint(19.1, 6.909, "some comment"),
                CreatePoint(44.8, 6.3, "yet, another, comment"),
                CreatePoint(70.1, 5.356, "another comment"),
                CreatePoint(82.4, 5.287)
            };
        }

        private static CrossSectionPoint CreatePoint(double distance, double elevation)
        {
            return CreatePoint(distance, elevation, string.Empty);
        }

        private static CrossSectionPoint CreatePoint(double distance, double elevation, string comment)
        {
            return new CrossSectionPoint
            {
                Distance = distance,
                Elevation = elevation,
                Comment = comment
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

        [Test]
        public void ParseFile_CrossSectionFileWithNegativeValues_CreatesExpectedPointsCollection()
        {
            var expectedPoints = CreateExpectedNegativeCrossSectionPoints();

            _stream = GetTestFile(NegativeValuesFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            AssertPointsMatchesExpected(result.Points, expectedPoints);
        }

        private static List<CrossSectionPoint> CreateExpectedNegativeCrossSectionPoints()
        {
            return new List<CrossSectionPoint>
            {
                CreatePoint(-1.5, -6),
                CreatePoint(-1.2, -6.905),
                CreatePoint(0, -2.1),
                CreatePoint(1, -1.4),
                CreatePoint(2.2, 2)
            };
        }
    }
}
