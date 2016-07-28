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
        private readonly IRelativeLocationInfo _mockRelativeLocation = TestHelpers.SetupMockRelativeLocation("At the gage");
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
            _mockLocationInfo.RelativeLocations.Returns(new List<IRelativeLocationInfo>
            {
                _mockRelativeLocation
            });
            _mockLocationInfo.Channels.Returns(new List<IChannelInfo>
            {
                _mockChannel
            });

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
                Channel = _mockChannel,
                RelativeLocation = _mockRelativeLocation,
                StartPoint = StartPointType.LeftEdgeOfWater,
                Comments = "Cross-section survey comments",
                Stage = 12.2,
                StageUnit = _mockUnit,
                DepthUnit = _mockUnit,
                DistanceUnit = _mockUnit,
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

        [Test]
        public void MapCrossSection_RelativeLocationExistsInLocation_DoesNotCallCreateNewRelativeLocation()
        {
            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.DidNotReceive().CreateNewRelativeLocation(Arg.Any<string>());
        }

        [Test]
        public void MapCrossSection_RelativeLocationDoesNotExistInLocation_CallsCreateNewRelativeLocation()
        {
            const string newRelativeLocation = "new relative location";
            _crossSectionSurvey.Fields[CrossSectionDataFields.RelativeLocation] = newRelativeLocation;

            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.Received(1).CreateNewRelativeLocation(newRelativeLocation);
        }

        [Test]
        public void MapCrossSection_EmptyRelativeLocationNameAndDefaultRelativeLocationExistsInLocation_ReturnsDefaultRelativeLocation()
        {
            var defaultRelativeLocation =
                TestHelpers.SetupMockRelativeLocation(CrossSectionParserConstants.DefaultRelativeLocationName);
            _mockLocationInfo.RelativeLocations.Returns(new List<IRelativeLocationInfo> { defaultRelativeLocation });
            _crossSectionSurvey.Fields[CrossSectionDataFields.RelativeLocation] = string.Empty;

            var actual = _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.DidNotReceive().CreateNewRelativeLocation(Arg.Any<string>());
            Assert.That(actual.RelativeLocation, Is.EqualTo(defaultRelativeLocation));
        }

        [Test]
        public void MapCrossSection_EmptyRelativeLocationNameAndDefaultRelativeLocationDoesNotExistInLocation_CallsCreateNewRelativeLocation()
        {
            _crossSectionSurvey.Fields[CrossSectionDataFields.RelativeLocation] = string.Empty;

            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.Received(1).CreateNewRelativeLocation(CrossSectionParserConstants.DefaultRelativeLocationName);
        }

        [Test]
        public void MapCrossSection_ChannelExistsInLocation_DoesNotCallCreateNewChannel()
        {
            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.DidNotReceive().CreateNewChannel(Arg.Any<string>());
        }

        [Test]
        public void MapCrossSection_ChannelDoesNotExistInLocation_CallsCreateNewChannel()
        {
            const string newChannel = "new channel";
            _crossSectionSurvey.Fields[CrossSectionDataFields.Channel] = newChannel;

            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.Received(1).CreateNewChannel(newChannel);
        }

        [Test]
        public void MapCrossSection_EmptyChannelNameAndDefaultChannelExistsInLocation_ReturnsDefaultChannel()
        {
            var defaultChannel =
                TestHelpers.SetupMockChannel(CrossSectionParserConstants.DefaultChannelName);
            _mockLocationInfo.Channels.Returns(new List<IChannelInfo> { defaultChannel });
            _crossSectionSurvey.Fields[CrossSectionDataFields.Channel] = string.Empty;

            var actual = _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.DidNotReceive().CreateNewChannel(Arg.Any<string>());
            Assert.That(actual.Channel, Is.EqualTo(defaultChannel));
        }

        [Test]
        public void MapCrossSection_EmptyChannelNameAndDefaultChannelDoesNotExistInLocation_CallsCreateNewChannel()
        {
            _crossSectionSurvey.Fields[CrossSectionDataFields.Channel] = string.Empty;

            _crossSectionMapper.MapCrossSection(_mockLocationInfo, _crossSectionSurvey);

            _mockLocationInfo.Received(1).CreateNewChannel(CrossSectionParserConstants.DefaultChannelName);
        }
    }
}
