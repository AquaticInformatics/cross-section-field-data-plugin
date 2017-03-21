using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
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

        private IParseContext _mockParseContext;
        private ICrossSectionPointMapper _mockCrossSectionPointMapper;
        private ILocationInfo _mockLocationInfo;
        private CrossSectionSurvey _crossSectionSurvey;

        private readonly IUnit _mockUnit = TestHelpers.SetupMockUnit("ft");
        private readonly IRelativeLocationInfo _mockRelativeLocation = TestHelpers.SetupMockRelativeLocation("At the Gage");
        private readonly IChannelInfo _mockChannel = TestHelpers.SetupMockChannel("Right overflow");

        [SetUp]
        public void TestSetup()
        {
            _mockParseContext = Substitute.For<IParseContext>();
            _mockParseContext.LengthUnits.Returns(new List<IUnit> { _mockUnit });

            _mockCrossSectionPointMapper = Substitute.For<ICrossSectionPointMapper>();
            _mockCrossSectionPointMapper.MapPoints(Arg.Any<List<CrossSectionPoint>>())
                .Returns(new List<PluginFramework.CrossSectionPoint>());

            _mockLocationInfo = Substitute.For<ILocationInfo>();

            _crossSectionMapper = new CrossSectionMapper(_mockParseContext, _mockCrossSectionPointMapper);

            _crossSectionSurvey = new CrossSectionSurvey
            {
                Fields = TestHelpers.CreateExpectedCrossSectionFields()
            };
        }

        [Test]
        public void MapCrossSection_CrossSectionWithExpectedMetadata_CreatesExpectedCrossSectionSurvey()
        {
            var expectedCrossSectionSurvey = CreateExpectedCrossSectionSurvey();

            var actual = _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            actual.ShouldBeEquivalentTo(expectedCrossSectionSurvey);
        }

        private PluginFramework.CrossSectionSurvey CreateExpectedCrossSectionSurvey()
        {
            return new PluginFramework.CrossSectionSurvey
            {
                StartTime = new DateTimeOffset(2001, 05, 08, 14, 32, 15, TimeSpan.FromHours(7)),
                EndTime = new DateTimeOffset(2001, 05, 08, 17, 12, 45, TimeSpan.FromHours(7)),
                Party = "Cross-Section Party",
                ChannelName = _mockChannel.ChannelName,
                RelativeLocation = _mockRelativeLocation,
                RelativeLocationName = _mockRelativeLocation.RelativeLocationName,
                StartPoint = StartPointType.LeftEdgeOfWater,
                Comments = "Cross-section survey comments",
                Stage = 12.2,
                StageUnit = _mockUnit,
                StageUnitId = _mockUnit.UnitId,
                DepthUnit = _mockUnit,
                DepthUnitId = _mockUnit.UnitId,
                DistanceUnit = _mockUnit,
                DistanceUnitId = _mockUnit.UnitId,
                CrossSectionPoints = new List<PluginFramework.CrossSectionPoint>()
            };
        }

        [Test]
        public void MapCrossSection_UnitDoesNotExistInParseContextLengthUnits_ReturnsGageHeightParameterDefaultUnit()
        {
            var mockGageHeightUnit = SetupGageHeightParameterWithDefaultUnit();

            _crossSectionSurvey.Fields[CrossSectionDataFields.Unit] = "SomeUnit";

            var actual = _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            Assert.That(actual.DepthUnit, Is.EqualTo(mockGageHeightUnit));
            Assert.That(actual.StageUnit, Is.EqualTo(mockGageHeightUnit));
            Assert.That(actual.DistanceUnit, Is.EqualTo(mockGageHeightUnit));
        }

        private IUnit SetupGageHeightParameterWithDefaultUnit()
        {
            var mockGageHeightUnit = TestHelpers.SetupMockUnit("m");
            var mockGageHeightParameter = Substitute.For<IParameter>();
            mockGageHeightParameter.DefaultUnit.Returns(mockGageHeightUnit);
            _mockParseContext.GageHeightParameter.Returns(mockGageHeightParameter);

            return mockGageHeightUnit;
        }

        [Test]
        public void MapCrossSection_CrossSectionPoints_CallsMapPointsOnCrossSectionPointMapper()
        {
            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockCrossSectionPointMapper.Received(1).MapPoints(Arg.Any<List<CrossSectionPoint>>());
        }
    }
}
