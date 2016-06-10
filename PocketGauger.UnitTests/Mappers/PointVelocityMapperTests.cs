using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    public class PointVelocityMapperTests
    {
        private IFixture _fixture;
        private IParseContext _context;
        private ILocationInfo _locationInfo;
        private IChannelInfo _channelInfo;

        private IVerticalMapper _mockVerticalMapper;
        private IPointVelocityMapper _mapper;
        private GaugingSummaryItem _gaugingSummaryItem;
        private DischargeActivity _dischargeActivity;

        private const int LocationUtcOffset = 3;

        [TestFixtureSetUp]
        public void SetupForAllTests()
        {
            SetupAutoFixture();

            _context = new ParseContextTestHelper().CreateMockParseContext();

            SetupMockLocationInfo();
            SetupMockVerticalMapper();

            _channelInfo = Substitute.For<IChannelInfo>();

            _gaugingSummaryItem = _fixture.Create<GaugingSummaryItem>();

            _mapper = new PointVelocityMapper(_context, _mockVerticalMapper);
        }

        private void SetupMockVerticalMapper()
        {
            _mockVerticalMapper = Substitute.For<IVerticalMapper>();
            _mockVerticalMapper.Map(null, null).ReturnsForAnyArgs(new List<Vertical>());
        }

        private void SetupAutoFixture()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            ParameterRegistrar.Register(_fixture);
            CollectionRegistrar.Register(_fixture);
            _fixture.Register<MeasurementConditionData>(() => new OpenWaterData());
        }

        private void SetupMockLocationInfo()
        {
            _locationInfo = Substitute.For<ILocationInfo>();

            _locationInfo.Channels.ReturnsForAnyArgs(new List<IChannelInfo> { _channelInfo });
            _locationInfo.UtcOffsetHours.ReturnsForAnyArgs(LocationUtcOffset);
        }

        [SetUp]
        public void SetupForEachTest()
        {
            _dischargeActivity = _fixture.Build<DischargeActivity>()
                .Without(activity => activity.DischargeSubActivities)
                .Create();
        }

        [TestCase(BankSide.Left, StartPointType.LeftEdgeOfWater)]
        [TestCase(BankSide.Right, StartPointType.RightEdgeOfWater)]
        [TestCase(null, StartPointType.Unspecified)]
        public void Map_BankSide_IsMappedToExpectedStartPointType(BankSide? bankSide, StartPointType expectedStartPoint)
        {
            _gaugingSummaryItem.StartBankProxy = bankSide?.ToString();

            var pointVelocityActivity = MapPointVelocityActivity();

            Assert.That(pointVelocityActivity.StartPoint, Is.EqualTo(expectedStartPoint));
        }

        private PointVelocityDischarge MapPointVelocityActivity()
        {
            return _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);
        }

        [TestCase(ParametersAndMethodsConstants.MeanSectionMonitoringMethod, PointVelocityMethodType.MeanSection)]
        [TestCase(ParametersAndMethodsConstants.MidSectionMonitoringMethod, PointVelocityMethodType.MidSection)]
        [TestCase(null, PointVelocityMethodType.Unknown)]
        public void Map_DischargeMethod_IsMappedToExpectedStartPointType(string monitoringMethodCode, PointVelocityMethodType expectedPointVelocityMethod)
        {
            _dischargeActivity.DischargeMethod.MethodCode.ReturnsForAnyArgs(monitoringMethodCode);

            var pointVelocityActivity = MapPointVelocityActivity();

            Assert.That(pointVelocityActivity.DischargeMethod, Is.EqualTo(expectedPointVelocityMethod));
        }

        private readonly IEnumerable<TestCaseData> _sampleDepthFlagTestCases = new List<TestCaseData>
        {
                          //Sample depths: .2    .4    .5    .6    .8  surface, bed
            new TestCaseData(Tuple.Create(true, true, true, true, true, true, false), PointVelocityObservationType.SixPoint),
            new TestCaseData(Tuple.Create(true, true, true, true, true, false, true), PointVelocityObservationType.SixPoint),
            new TestCaseData(Tuple.Create(true, true, true, true, true, false, false), PointVelocityObservationType.FivePoint),
            new TestCaseData(Tuple.Create(true, false, false, true, true, false, false), PointVelocityObservationType.OneAtPointTwoPointSixAndPointEight),
            new TestCaseData(Tuple.Create(true, false, false, false, true, false, false), PointVelocityObservationType.OneAtPointTwoAndPointEight),
            new TestCaseData(Tuple.Create(false, false, false, true, false, false, false), PointVelocityObservationType.OneAtPointSix),
            new TestCaseData(Tuple.Create(false, false, true, false, false, false, false), PointVelocityObservationType.OneAtPointFive),
            new TestCaseData(Tuple.Create(false, false, false, false, false, true, false), PointVelocityObservationType.Surface),
            new TestCaseData(Tuple.Create(true, true, false, false, false, false, false), PointVelocityObservationType.Unknown),
            new TestCaseData(Tuple.Create(false, true, false, false, false, false, false), PointVelocityObservationType.Unknown),
            new TestCaseData(Tuple.Create(true, true, true, true, true, true, true), PointVelocityObservationType.Unknown)
        };

        [TestCaseSource(nameof(_sampleDepthFlagTestCases))]
        public void PointVelocityObservationTypeMap_SampleDepthFlags_IsMappedToExpectedStartPointType(
            Tuple<bool, bool, bool, bool, bool, bool, bool> sampleFlags, PointVelocityObservationType expectedVelocityObservationMethod)
        {
            SetSampleFlags(sampleFlags);

            var pointVelocityActivity = MapPointVelocityActivity();

            Assert.That(pointVelocityActivity.VelocityObservationMethod, Is.EqualTo(expectedVelocityObservationMethod));
        }

        private void SetSampleFlags(Tuple<bool, bool, bool, bool, bool, bool, bool> sampleFlags)
        {
            _gaugingSummaryItem.SampleAt2Proxy = SerializeBool(sampleFlags.Item1);
            _gaugingSummaryItem.SampleAt4Proxy = SerializeBool(sampleFlags.Item2);
            _gaugingSummaryItem.SampleAt5Proxy = SerializeBool(sampleFlags.Item3);
            _gaugingSummaryItem.SampleAt6Proxy = SerializeBool(sampleFlags.Item4);
            _gaugingSummaryItem.SampleAt8Proxy = SerializeBool(sampleFlags.Item5);
            _gaugingSummaryItem.SampleAtSurfaceProxy = SerializeBool(sampleFlags.Item6);
            _gaugingSummaryItem.SampleAtBedProxy = SerializeBool(sampleFlags.Item7);
        }

        private static string SerializeBool(bool value)
        {
            return BooleanHelper.Serialize(value);
        }

        private readonly IEnumerable<TestCaseData> _nonBridgeGaugingMethodToMeterSuspensionAndExpectedDeploymentTypeTestCases = new List<TestCaseData>
        {
            new TestCaseData(GaugingMethod.BoatWithHandlines, Tuple.Create(MeterSuspensionType.Handline, DeploymentMethodType.Boat)),
            new TestCaseData(GaugingMethod.BoatWithRods, Tuple.Create(MeterSuspensionType.RoundRod, DeploymentMethodType.Boat)),
            new TestCaseData(GaugingMethod.BoatWithWinch, Tuple.Create(MeterSuspensionType.PackReel, DeploymentMethodType.Boat)),
            new TestCaseData(GaugingMethod.Cableway, Tuple.Create(MeterSuspensionType.Unspecified, DeploymentMethodType.Cableway)),
            new TestCaseData(GaugingMethod.Waded, Tuple.Create(MeterSuspensionType.Unspecified, DeploymentMethodType.Wading)),
            new TestCaseData(GaugingMethod.Ice, Tuple.Create(MeterSuspensionType.IceSurfaceMount, DeploymentMethodType.Ice)),
            new TestCaseData(GaugingMethod.Suspended, Tuple.Create(MeterSuspensionType.Unspecified, DeploymentMethodType.Unspecified)),
            new TestCaseData(null, Tuple.Create(MeterSuspensionType.Unspecified, DeploymentMethodType.Unspecified))
        };

        [TestCaseSource(nameof(_nonBridgeGaugingMethodToMeterSuspensionAndExpectedDeploymentTypeTestCases))]
        public void Map_NonBridgeGaugingMethod_IsMappedToExpectedMeterSuspensionAndDeploymentType(GaugingMethod? gaugingMethod, 
            Tuple<MeterSuspensionType, DeploymentMethodType> expectedMeterAndDeploymentType)
        {
            _gaugingSummaryItem.GaugingMethodProxy = gaugingMethod.ToString();

            var pointVelocityDischarge = MapPointVelocityActivity();

            AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(pointVelocityDischarge.ChannelMeasurement, 
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        private static void AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(DischargeChannelMeasurement channelMeasurement, 
            MeterSuspensionType meterSuspensionType, DeploymentMethodType expectedMeterAndDeploymentType)
        {
            Assert.That(channelMeasurement.MeterSuspension, Is.EqualTo(meterSuspensionType));
            Assert.That(channelMeasurement.DeploymentMethod, Is.EqualTo(expectedMeterAndDeploymentType));
        }

        private readonly IEnumerable<TestCaseData> _bridgeGaugingMethodAndBankSideToExpectedMeterSuspensionAndDeploymentTypeTestCases = new List<TestCaseData>
        {
            new TestCaseData(GaugingMethod.BridgeWithHandlines, BankSide.Left, Tuple.Create(MeterSuspensionType.Handline, DeploymentMethodType.BridgeDownstreamSide)),
            new TestCaseData(GaugingMethod.BridgeWithRods, BankSide.Left, Tuple.Create(MeterSuspensionType.RoundRod, DeploymentMethodType.BridgeDownstreamSide)),
            new TestCaseData(GaugingMethod.BridgeWithWinch, BankSide.Left, Tuple.Create(MeterSuspensionType.PackReel, DeploymentMethodType.BridgeDownstreamSide)),

            new TestCaseData(GaugingMethod.BridgeWithHandlines, BankSide.Right, Tuple.Create(MeterSuspensionType.Handline, DeploymentMethodType.BridgeUpstreamSide)),
            new TestCaseData(GaugingMethod.BridgeWithRods, BankSide.Right, Tuple.Create(MeterSuspensionType.RoundRod, DeploymentMethodType.BridgeUpstreamSide)),
            new TestCaseData(GaugingMethod.BridgeWithWinch, BankSide.Right, Tuple.Create(MeterSuspensionType.PackReel, DeploymentMethodType.BridgeUpstreamSide)),

            new TestCaseData(GaugingMethod.BridgeWithHandlines, null, Tuple.Create(MeterSuspensionType.Handline, DeploymentMethodType.Unspecified)),
            new TestCaseData(GaugingMethod.BridgeWithRods, null, Tuple.Create(MeterSuspensionType.RoundRod, DeploymentMethodType.Unspecified)),
            new TestCaseData(GaugingMethod.BridgeWithWinch, null, Tuple.Create(MeterSuspensionType.PackReel, DeploymentMethodType.Unspecified)),
        };

        [TestCaseSource(nameof(_bridgeGaugingMethodAndBankSideToExpectedMeterSuspensionAndDeploymentTypeTestCases))]
        public void Map_BridgeGaugingMethod_IsMappedToExpectedMeterSuspensionAndDeploymentType(GaugingMethod gaugingMethod, BankSide? bankSide,
            Tuple<MeterSuspensionType, DeploymentMethodType> expectedMeterAndDeploymentType)
        {
            _gaugingSummaryItem.GaugingMethodProxy = gaugingMethod.ToString();
            _gaugingSummaryItem.StartBankProxy = bankSide.ToString();

            var pointVelocityDischarge = MapPointVelocityActivity();

            AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(pointVelocityDischarge.ChannelMeasurement,
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedPointVelocityDischarge()
        {
            var expectedDischargeActivity = CreateExpectedPointVelocityDischarge();

            var pointVelocityDischarge = MapPointVelocityActivity();

            pointVelocityDischarge.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.ChannelMeasurement)
                .Excluding(activity => activity.Verticals)
                .Excluding(activity => activity.DischargeMethod)
                .Excluding(activity => activity.VelocityObservationMethod)
                .Excluding(activity => activity.StartPoint));
        }

        private PointVelocityDischarge CreateExpectedPointVelocityDischarge()
        {
            return new PointVelocityDischarge
            {
                Area = _gaugingSummaryItem.Area,
                AreaUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.AreaParameterId),
                MeasurementConditions = MeasurementCondition.OpenWater,
                TaglinePointUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                DistanceToMeterUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                VelocityAverage = _gaugingSummaryItem.MeanVelocity,
                VelocityAverageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
                WidthUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.WidthParameterId),
                AscendingSegmentDisplayOrder = true
            };
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedDischargeChannelMeasurement()
        {
            var expectedDischargeActivity = CreateExpectedDischargeChannelMeasurement();

            var pointVelocityDischarge = MapPointVelocityActivity();

            pointVelocityDischarge.ChannelMeasurement.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.MeterSuspension)
                .Excluding(activity => activity.DeploymentMethod));
        }

        private DischargeChannelMeasurement CreateExpectedDischargeChannelMeasurement()
        {
            return new DischargeChannelMeasurement
            {
                StartTime = _dischargeActivity.StartTime,
                EndTime = _dischargeActivity.EndTime,
                Discharge = _gaugingSummaryItem.Flow.GetValueOrDefault(),
                Channel = _channelInfo,
                Comments = _gaugingSummaryItem.Comments,
                Party = _gaugingSummaryItem.ObserversName,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                MonitoringMethod = _dischargeActivity.DischargeMethod,
                DistanceToGageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId)
            };
        }

        [Test]
        public void Map_RetrievesVerticalsFromVerticalMapper()
        {
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            _mockVerticalMapper.Received().Map(_gaugingSummaryItem, pointVelocityDischarge.ChannelMeasurement);
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsMeanObservationDurationAsNull()
        {
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);
            
            Assert.That(pointVelocityDischarge.MeanObservationDuration, Is.Null);
        }

        [Test]
        public void Map_VelocityObservationWithIntervals_CalculatedMeanObservationDurationAsExpected()
        {
            const double observationIntervalOffset = 50;
            const double expectedMeanDuration = 100;

            var observations = CreateVelocityObservationWithEquallySpacedInterval(observationIntervalOffset);

            var velocityObservations = _fixture.Build<VelocityObservation>()
                .With(observation => observation.Observations, observations)
                .Create();

            var verticals = _fixture.Build<Vertical>()
                .With(vertical => vertical.VelocityObservation, velocityObservations)
                .CreateMany(1);

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MeanObservationDuration, Is.EqualTo(expectedMeanDuration));
        }

        private void SetupVerticalMapperToReturn(IEnumerable<Vertical> verticals)
        {
            _mockVerticalMapper.Map(Arg.Any<GaugingSummaryItem>(), Arg.Any<DischargeChannelMeasurement>())
                .Returns(verticals.ToList());
        }

        private ICollection<VelocityDepthObservation> CreateVelocityObservationWithEquallySpacedInterval(double intervalForEachObservation)
        {
            var observations = new List<VelocityDepthObservation>();

            for (var i = 1; i <= 3; i++)
            {
                var observation = _fixture.Build<VelocityDepthObservation>()
                    .With(obs => obs.ObservationInterval, i * intervalForEachObservation)
                    .Create();

                observations.Add(observation);
            }

            return observations;
        }

        [Test]
        public void Map_VelocityObservationWithoutIntervals_CalculatesMeanObservationDurationAsExpected()
        {
            var observations = _fixture.Build<VelocityDepthObservation>()
                .Without(obs => obs.ObservationInterval)
                .CreateMany()
                .ToList();

            var velocityObservations = _fixture.Build<VelocityObservation>()
                .With(observation => observation.Observations, observations)
                .Create();

            var verticals = _fixture.Build<Vertical>()
                .With(vertical => vertical.VelocityObservation, velocityObservations)
                .CreateMany(1);

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MeanObservationDuration, Is.EqualTo(null));
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsWidthToNull()
        {
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.Width, Is.Null);
        }

        [Test]
        public void Map_PointVelocityWithVerticals_CalculatesWidthAsExpected()
        {
            const double segmentWidth = 25;
            const double expectedTotalWidth = 75;

            var segment = _fixture.Build<Segment>()
                .With(seg => seg.Width, segmentWidth)
                .Create();

            var verticals = _fixture.Build<Vertical>()
                .With(vertical => vertical.Segment, segment)
                .CreateMany();

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.Width, Is.EqualTo(expectedTotalWidth));
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsAscendingDisplayOrderToTrue()
        {
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.True);
        }

        [Test]
        public void Map_TaglinePositionForFirstVerticalIsSmallerThanThatOfLastVertical_SetsAscendingDisplayOrderToTrue()
        {
            var verticals = _fixture.CreateMany<Vertical>(2).ToList();
            verticals.First().TaglinePosition = 0;
            verticals.Last().TaglinePosition = 10;

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.True);
        }

        [Test]
        public void Map_TaglinePositionForFirstVerticalIsLargerThanThatOfLastVertical_SetsAscendingDisplayOrderToFalse()
        {
            var verticals = _fixture.CreateMany<Vertical>(2).ToList();
            verticals.First().TaglinePosition = 10;
            verticals.Last().TaglinePosition = 0;

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.False);
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsMaximumSegmentDischargeToNull()
        {
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MaximumSegmentDischarge, Is.Null);
        }

        [Test]
        public void Map_PointVelocityWithVerticals_SetsMaximumSegmentDischargeToVerticalWithLargestTotalDischargePortion()
        {
            var verticals = _fixture.CreateMany<Vertical>().ToList();
            
            SetupVerticalMapperToReturn(verticals);

            var expectedMaximumSegmentDischarge = verticals
                .OrderBy(vertical => vertical.Segment.TotalDischargePortion)
                .Last()
                .Segment
                .TotalDischargePortion;
            
            var pointVelocityDischarge = _mapper.Map(_channelInfo, _gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MaximumSegmentDischarge, Is.EqualTo(expectedMaximumSegmentDischarge));
        }
    }
}
