using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.UnitTests.TestData;
using PluginFramework = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Mappers
{
    [TestFixture]
    public class ParsedResultMapperTests
    {
        private IFixture _fixture;
        private IParsedResultMapper _parsedResultMapper;

        private IParseContext _mockParseContext;
        private ICrossSectionMapper _mockCrossSectionMapper;
        private CrossSectionSurvey _crossSectionSurvey;

        private ILocationInfo _mockLocationInfo;
        private IFieldVisitInfo _mockFieldVisitInfo;
        private PluginFramework.CrossSectionSurvey _pluginFrameworkCrossSection;

        [SetUp]
        public void TestSetup()
        {
            _fixture = new Fixture();
            TestHelpers.RegisterMockTypes(_fixture);
            _crossSectionSurvey = new CrossSectionSurvey { Fields = TestHelpers.CreateExpectedCrossSectionFields() };

            _mockLocationInfo = TestHelpers.SetupMockLocationInfo(_crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Location));
            _mockFieldVisitInfo = Substitute.For<IFieldVisitInfo>();

            _mockParseContext = Substitute.For<IParseContext>();
            _mockParseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns(_mockLocationInfo);
            _mockParseContext.TargetLocation.Returns(_mockLocationInfo);
            _mockParseContext.TargetFieldVisit.Returns((IFieldVisitInfo)null);

            _mockCrossSectionMapper = Substitute.For<ICrossSectionMapper>();
            _pluginFrameworkCrossSection = _fixture.Build<PluginFramework.CrossSectionSurvey>()
                .With(survey => survey.CrossSectionPoints, _fixture.CreateMany<PluginFramework.CrossSectionPoint>().ToList())
                .Create();
            _mockCrossSectionMapper.MapCrossSection(Arg.Any<ILocationInfo>(), Arg.Any<CrossSectionSurvey>())
                .Returns(_pluginFrameworkCrossSection);

            _parsedResultMapper = new ParsedResultMapper(_mockParseContext, _mockCrossSectionMapper);
        }

        [Test]
        public void CreateParsedResult_CrossSectionLocationDoesNotExist_Throws()
        {
            _mockParseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns((ILocationInfo)null);

            TestDelegate testDelegate = () => _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>().With.Message.Contains("does not exist"));
        }

        [Test]
        public void CreateParsedResult_LocationInCrossSectionFileExists_CallsMapCrossSection()
        {
            _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            _mockCrossSectionMapper.Received(1).MapCrossSection(Arg.Any<ILocationInfo>(), Arg.Any<CrossSectionSurvey>());
        }

        [Test]
        public void CreateParsedResult_LocationExistsButDoesNotMatchLocationOfFieldVisit_Throws()
        {
            var location = TestHelpers.SetupMockLocationInfo(_fixture.Create<string>());
            _mockParseContext.TargetLocation.Returns(location);
            SetupFieldVisitUpload();

            TestDelegate testDelegate = () => _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>()
                .With.Message.Contains("does not match the identifier in the file"));
        }

        private void SetupFieldVisitUpload()
        {
            _mockParseContext.TargetFieldVisit.Returns(_mockFieldVisitInfo);
        }

        [Test]
        public void CreateParsedResult_NoExistingVisitsInLocation_ReturnsNewVisitParseResponse()
        {
            _mockLocationInfo.FindLocationFieldVisitsInTimeRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                .Returns(new List<IFieldVisitInfo>());

            var parsedResult = _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            var newFieldVisit = parsedResult as NewFieldVisit;

            Assert.That(newFieldVisit, Is.Not.Null);
            AssertNewFieldVisitIsCreatedWithExpectedProperties(newFieldVisit);
        }

        private void AssertNewFieldVisitIsCreatedWithExpectedProperties(NewFieldVisit newFieldVisit)
        {
            var visit = newFieldVisit.FieldVisit;

            Assert.That(visit.StartDate, Is.EqualTo(_pluginFrameworkCrossSection.StartTime));
            Assert.That(visit.EndDate, Is.EqualTo(_pluginFrameworkCrossSection.EndTime));
            Assert.That(visit.Party, Is.EqualTo(_pluginFrameworkCrossSection.Party));
            Assert.That(visit.CrossSectionSurveys, Has.Member(_pluginFrameworkCrossSection));
        }

        [Test]
        public void CreateParsedResult_LocationAlreadyHasVisitWithinCrossSectionTime_Throws()
        {
            _mockLocationInfo.FindLocationFieldVisitsInTimeRange(_pluginFrameworkCrossSection.StartTime,
                _pluginFrameworkCrossSection.EndTime).Returns(new List<IFieldVisitInfo> { _mockFieldVisitInfo} );

            TestDelegate testDelegate = () => _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>()
                .With.Message.Contains("Visit within the time range of the Cross-Section already exists"));
        }

        [Test]
        public void CreateParsedResult_FieldDataUpload_RetunsNewCrossSectionSurveyResponse()
        {
            SetupFieldVisitUpload();

            var parsedResult = _parsedResultMapper.CreateParsedResult(_crossSectionSurvey);

            var newCrossSectionSurvey = parsedResult as NewCrossSectionSurvey;

            Assert.That(newCrossSectionSurvey, Is.Not.Null);
            AssertNewCrossSectionSurveyIsCreatedWithExpectedProperties(newCrossSectionSurvey);
        }

        private void AssertNewCrossSectionSurveyIsCreatedWithExpectedProperties(NewCrossSectionSurvey newCrossSectionSurvey)
        {
            Assert.That(newCrossSectionSurvey.CrossSectionSurvey, Is.EqualTo(_pluginFrameworkCrossSection));
            Assert.That(newCrossSectionSurvey.FieldVisit, Is.EqualTo(_mockFieldVisitInfo));
        }
    }
}
