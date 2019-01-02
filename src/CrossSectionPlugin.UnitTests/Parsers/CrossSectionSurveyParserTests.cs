using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;
using CrossSectionPlugin.Interfaces;
using CrossSectionPlugin.Model;
using CrossSectionPlugin.Parsers;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace CrossSectionPlugin.UnitTests.Parsers
{
    [TestFixture]
    public class CrossSectionSurveyParserTests
    {
        private const string NamespacePrefix = "CrossSectionPlugin.UnitTests.TestData.";

        private const string ValidCrossSectionFilePath = NamespacePrefix + "CrossSection.csv";
        private const string DuplicateMetadataCrossSectionFilePath = NamespacePrefix + "CrossSectionWithDuplicateLocationFields.csv";
        private const string NegativeValuesFilePath = NamespacePrefix + "CrossSectionWithNegativeValues.csv";
        private const string V1FileWithPointOrderFilePath = NamespacePrefix + "V1CrossSectionWithPointOrder.csv";
        private const string V2CrossSectionFilePath = NamespacePrefix + "V2CrossSection.csv";
        private const string V2WithoutPointOrderFilePath = NamespacePrefix + "V2CrossSectionWithoutPointOrder.csv";

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

            void ParseWithNonCrossSectionFile() => _crossSectionParser.ParseFile(_stream);

            Assert.That(ParseWithNonCrossSectionFile,
                Throws.Exception.TypeOf<CrossSectionCsvFormatException>().With.Message.Contains("not an AQUARIUS Cross-Section"));
        }

        private static readonly IEnumerable<string> ValidFileHeaders = new[]
        {
            CrossSectionParserConstants.Header + " " + CrossSectionParserConstants.DefaultVersion,
            CrossSectionParserConstants.Header + CrossSectionParserConstants.DefaultVersion,
            CrossSectionParserConstants.Header.ToLowerInvariant() + " " + CrossSectionParserConstants.DefaultVersion,
            CrossSectionParserConstants.Header.ToUpperInvariant() + " " + CrossSectionParserConstants.DefaultVersion,
            CrossSectionParserConstants.Header + CrossSectionParserConstants.DefaultVersion + "Some Other Text",
            CrossSectionParserConstants.Header + "Some Other Text" + CrossSectionParserConstants.DefaultVersion
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
            new TestCaseData("1.4"),
            new TestCaseData("version2.24.5.4")
        };

        [TestCaseSource(nameof(VersionTests))]
        public void ParseFile_ValidCrossSectionFile_ParsesFileVersionPropertyFromHeader(string versionString)
        {
            _stream = SetupMemoryStreamWithText(CrossSectionParserConstants.Header + versionString);

            var parsedResults = _crossSectionParser.ParseFile(_stream);

            versionString.Should().Contain(parsedResults.CsvFileVersion.ToString());
        }

        [Test]
        public void ParseFile_UnknownHeaderVersionString_Throws()
        {

            _stream = SetupMemoryStreamWithText(CrossSectionParserConstants.Header + "versionTwoPointThree");

            void ParseWithUnknownCrossSectionFileVersion() => _crossSectionParser.ParseFile(_stream);

            Assert.That(ParseWithUnknownCrossSectionFileVersion,
                Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>()
                    .With.Message.EqualTo(CrossSectionSurveyParser.CannotParseFileVersion));
        }

        [Test]
        public void ParseFile_MissingVersionFromHeaderString_Throws()
        {
            _stream = SetupMemoryStreamWithText(CrossSectionParserConstants.Header);

            void ParseWithMissingCrossSectionFileVersion() => _crossSectionParser.ParseFile(_stream);

            Assert.That(ParseWithMissingCrossSectionFileVersion,
                Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>()
                    .With.Message.EqualTo(CrossSectionSurveyParser.CannotParseFileVersion));
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
        public void ParseFile_FileHasDuplicateLocationRecords_ThrowsException()
        {
            _stream = GetTestFile(DuplicateMetadataCrossSectionFilePath);

            void ParseFileWithDuplicateLocationRecords() => _crossSectionParser.ParseFile(_stream);

            Assert.That(ParseFileWithDuplicateLocationRecords,
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

        private static List<CrossSectionPointV1> CreateExpectedCrossSectionPoints()
        {
            return new List<CrossSectionPointV1>
            {
                CreatePoint(0, 7.467),
                CreatePoint(19.1, 6.909, "some comment"),
                CreatePoint(44.8, 6.3, "yet, another, comment"),
                CreatePoint(70.1, 5.356, "another comment"),
                CreatePoint(82.4, 5.287)
            };
        }

        private static CrossSectionPointV1 CreatePoint(double distance, double elevation)
        {
            return CreatePoint(distance, elevation, string.Empty);
        }

        private static CrossSectionPointV1 CreatePoint(double distance, double elevation, string comment)
        {
            return new CrossSectionPointV1
            {
                Distance = distance,
                Elevation = elevation,
                Comment = comment
            };
        }

        private static void AssertPointsMatchesExpected(List<ICrossSectionPoint> actualPoints, List<CrossSectionPointV1> expectedPoints)
        {
            foreach (var point in actualPoints.Zip(expectedPoints, Tuple.Create))
            {
                var actual = point.Item1 as CrossSectionPointV1;
                var expectation = point.Item2;

                AssertPointIsEqual(actual, expectation);
            }
        }

        private static void AssertPointIsEqual(CrossSectionPointV1 actual, CrossSectionPointV1 expectation)
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

        private static List<CrossSectionPointV1> CreateExpectedNegativeCrossSectionPoints()
        {
            return new List<CrossSectionPointV1>
            {
                CreatePoint(-1.5, -6),
                CreatePoint(-1.2, -6.905),
                CreatePoint(0, -2.1),
                CreatePoint(1, -1.4),
                CreatePoint(2.2, 2)
            };
        }

        [Test]
        public void ParseFile_VersionOneFileWithPointOrderColumn_Throws()
        {
            _stream = GetTestFile(V1FileWithPointOrderFilePath);

            void ParseV1FileWithPointOrderColumn() => _crossSectionParser.ParseFile(_stream);

            Assert.That(ParseV1FileWithPointOrderColumn,
                Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>().With.Message.Contains("does not support \"PointOrder\" as a column"));
        }

        [Test]
        public void ParseFile_VersionTwoFile_CreatesExpected()
        {
            var expectedCrossSectionFields = TestData.TestHelpers.CreateExpectedCrossSectionFields();
            var expectedPoints = CreateExpectedV2Points();

            _stream = GetTestFile(V2CrossSectionFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            AssertCrossSectionMatchesExpected(result, expectedCrossSectionFields, expectedPoints);
        }

        private static List<CrossSectionPointV2> CreateExpectedV2Points()
        {
            return new List<CrossSectionPointV2>
            {
                new CrossSectionPointV2 { PointOrder = 2, Distance = 2, Elevation = 1, Comment = "comment" },
                new CrossSectionPointV2 { PointOrder = 1, Distance = 3, Elevation = 4, Comment = string.Empty },
                new CrossSectionPointV2 { PointOrder = 3, Distance = 4, Elevation = 5, Comment = string.Empty }
            };
        }

        private static void AssertCrossSectionMatchesExpected(CrossSectionSurvey result,
            IDictionary<string, string> expectedCrossSectionFields, List<CrossSectionPointV2> expectedPoints)
        {
            result.Fields.ShouldBeEquivalentTo(expectedCrossSectionFields);

            result.Points.Should().HaveCount(expectedPoints.Count);
            for (var i = 0; i < result.Points.Count; i++)
            {
                var actual = result.Points[i] as CrossSectionPointV2;
                var expected = expectedPoints[i];

                Assert.That(actual, Is.Not.Null);
                actual.PointOrder.Should().Be(expected.PointOrder);
                actual.Distance.Should().Be(expected.Distance);
                actual.Elevation.Should().Be(expected.Elevation);
                actual.Comment.Should().Be(expected.Comment);
            }
        }

        [Test]
        public void ParseFile_VersionTwoFileWithoutPointOrder_CreatesExpected()
        {
            var expectedCrossSectionFields = TestData.TestHelpers.CreateExpectedCrossSectionFields();
            var expectedPoints = CreateExpectedV2PointsWithInferredPointOrder();

            _stream = GetTestFile(V2WithoutPointOrderFilePath);

            var result = _crossSectionParser.ParseFile(_stream);

            AssertCrossSectionMatchesExpected(result, expectedCrossSectionFields, expectedPoints);
        }

        private static List<CrossSectionPointV2> CreateExpectedV2PointsWithInferredPointOrder()
        {
            return new List<CrossSectionPointV2>
            {
                new CrossSectionPointV2 { PointOrder = 1, Distance = 3, Elevation = 4, Comment = string.Empty },
                new CrossSectionPointV2 { PointOrder = 2, Distance = 2, Elevation = 1, Comment = "comment" },
                new CrossSectionPointV2 { PointOrder = 3, Distance = 4, Elevation = 5, Comment = string.Empty }
            };
        }
    }
}
