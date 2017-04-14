using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.UnitTests.TestData;
using PluginFramework = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Mappers
{
    [TestFixture]
    public class CrossSectionMapperTests
    {
        private ICrossSectionMapper _crossSectionMapper;

        private ICrossSectionPointMapper _mockCrossSectionPointMapper;
        private CrossSectionSurvey _crossSectionSurvey;

        [SetUp]
        public void TestSetup()
        {
            _mockCrossSectionPointMapper = Substitute.For<ICrossSectionPointMapper>();
            _mockCrossSectionPointMapper.MapPoints(Arg.Any<List<CrossSectionPoint>>())
                .Returns(new List<PluginFramework.CrossSectionPoint>());

            _crossSectionMapper = new CrossSectionMapper(_mockCrossSectionPointMapper);

            _crossSectionSurvey = new CrossSectionSurvey
            {
                Fields = TestHelpers.CreateExpectedCrossSectionFields()
            };
        }

        [Test]
        public void MapCrossSection_CrossSectionWithExpectedMetadata_CreatesExpectedCrossSectionSurvey()
        {
            var expectedCrossSectionSurvey = CreateExpectedCrossSectionSurvey();

            var actual = _crossSectionMapper.MapCrossSection(_crossSectionSurvey);

            actual.ShouldBeEquivalentTo(expectedCrossSectionSurvey);
        }

        private PluginFramework.CrossSectionSurvey CreateExpectedCrossSectionSurvey()
        {
            DateTimeOffset startTime = new DateTimeOffset(2001, 05, 08, 14, 32, 15, TimeSpan.FromHours(7));
            DateTimeOffset endTime = new DateTimeOffset(2001, 05, 08, 17, 12, 45, TimeSpan.FromHours(7));
            string party = "Cross-Section Party";

            return new PluginFramework.CrossSectionSurvey(new DateTimeInterval(startTime, endTime), party)
            {
                ChannelName = "Right overflow",
                RelativeLocationName = "At the Gage",
                StartPoint = StartPointType.LeftEdgeOfWater,
                Comments = "Cross-section survey comments",
                StageMeasurement = new Measurement(12.2, "ft"),
                DepthUnitId = "ft",
                DistanceUnitId = "ft",
                CrossSectionPoints = new List<PluginFramework.CrossSectionPoint>()
            };
        }

        [Test]
        public void MapCrossSection_CrossSectionPoints_CallsMapPointsOnCrossSectionPointMapper()
        {
            _crossSectionMapper.MapCrossSection(_crossSectionSurvey);

            _mockCrossSectionPointMapper.Received(1).MapPoints(Arg.Any<List<CrossSectionPoint>>());
        }
    }
}
