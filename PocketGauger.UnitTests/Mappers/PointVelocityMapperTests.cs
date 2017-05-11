using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.Verticals;
using Server.BusinessInterfaces.FieldDataPluginCore.UnitTests.TestHelpers;
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

        private IVerticalMapper _mockVerticalMapper;
        private IPointVelocityMapper _mapper;
        private GaugingSummaryItem _gaugingSummaryItem;
        private DischargeActivity _dischargeActivity;

        [TestFixtureSetUp]
        public void SetupForAllTests()
        {
            SetupAutoFixture();

            SetupMockVerticalMapper();

            _gaugingSummaryItem = _fixture.Create<GaugingSummaryItem>();

            _mapper = new PointVelocityMapper(_mockVerticalMapper);
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
            _fixture.AddFieldDataPluginCoreTestingExtensions();
            CollectionRegistrar.Register(_fixture);
            _fixture.Register<MeasurementConditionData>(() => new OpenWaterData());
        }

        [SetUp]
        public void SetupForEachTest()
        {
            _dischargeActivity = _fixture.Build<DischargeActivity>()
                .Without(activity => activity.ChannelMeasurements)
                .Create();
        }

        [TestCase(BankSide.Left, StartPointType.LeftEdgeOfWater)]
        [TestCase(BankSide.Right, StartPointType.RightEdgeOfWater)]
        [TestCase(null, StartPointType.Unspecified)]
        public void Map_BankSide_IsMappedToExpectedStartPointType(BankSide? bankSide, StartPointType expectedStartPoint)
        {
            _gaugingSummaryItem.StartBankProxy = bankSide?.ToString();

            var dischargeSection = MapToManualGaugingDischargeSection();

            Assert.That(dischargeSection.StartPoint, Is.EqualTo(expectedStartPoint));
        }

        private ManualGaugingDischargeSection MapToManualGaugingDischargeSection()
        {
            return _mapper.Map(_gaugingSummaryItem, _dischargeActivity);
        }

        [TestCase(FlowCalculationMethod.Mean, DischargeMethodType.MeanSection)]
        [TestCase(FlowCalculationMethod.Mid, DischargeMethodType.MidSection)]
        [TestCase(null, DischargeMethodType.Unknown)]
        public void Map_FlowCalculationMethod_IsMappedToExpectedDischargeMethodType(FlowCalculationMethod? flowCalculationMethod, DischargeMethodType expectedPointVelocityMethod)
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = flowCalculationMethod?.ToString();

            var dischargeSection = MapToManualGaugingDischargeSection();

            Assert.That(dischargeSection.DischargeMethod, Is.EqualTo(expectedPointVelocityMethod));
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

            var dischargeSection = MapToManualGaugingDischargeSection();

            Assert.That(dischargeSection.VelocityObservationMethod, Is.EqualTo(expectedVelocityObservationMethod));
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

            var pointVelocityDischarge = MapToManualGaugingDischargeSection();

            AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(pointVelocityDischarge.Channel,
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        private static void AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(Channel channel,
            MeterSuspensionType meterSuspensionType, DeploymentMethodType expectedMeterAndDeploymentType)
        {
            Assert.That(channel.MeterSuspension, Is.EqualTo(meterSuspensionType));
            Assert.That(channel.DeploymentMethod, Is.EqualTo(expectedMeterAndDeploymentType));
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

            var pointVelocityDischarge = MapToManualGaugingDischargeSection();

            AssertChannelMeasurementHasExpectedSuspensionAndDeploymentType(pointVelocityDischarge.Channel,
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedPointVelocityDischarge()
        {
            var expectedDischargeActivity = CreateExpectedManualGaugingDischargeSection();

            var pointVelocityDischarge = MapToManualGaugingDischargeSection();

            pointVelocityDischarge.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.Channel)
                .Excluding(activity => activity.Verticals)
                .Excluding(activity => activity.DischargeMethod)
                .Excluding(activity => activity.VelocityObservationMethod)
                .Excluding(activity => activity.StartPoint));
        }

        private ManualGaugingDischargeSection CreateExpectedManualGaugingDischargeSection()
        {
            return new ManualGaugingDischargeSection
            {
                Area = _gaugingSummaryItem.Area,
                AreaUnitId = "m^2",
                MeasurementConditions = MeasurementCondition.OpenWater,
                TaglinePointUnitId = "m",
                DistanceToMeterUnitId = "m",
                VelocityAverage = _gaugingSummaryItem.MeanVelocity,
                VelocityAverageUnitId = "m/s",
                WidthUnitId = "m",
                AscendingSegmentDisplayOrder = true
            };
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedDischargeChannelMeasurement()
        {
            var expectedDischargeActivity = CreateExpectedChannel();

            var pointVelocityDischarge = MapToManualGaugingDischargeSection();

            pointVelocityDischarge.Channel.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.MeterSuspension)
                .Excluding(activity => activity.DeploymentMethod));
        }

        private Channel CreateExpectedChannel()
        {
            return new Channel
            {
                StartTime = _dischargeActivity.MeasurementPeriod.Start,
                EndTime = _dischargeActivity.MeasurementPeriod.End,
                Discharge = _gaugingSummaryItem.Flow.GetValueOrDefault(),
                ChannelName = "Main",
                Comments = _gaugingSummaryItem.Comments,
                Party = _gaugingSummaryItem.ObserversName,
                DischargeUnitId = "m^3/s",
                DistanceToGageUnitId = "m"
            };
        }

        [Test]
        public void Map_RetrievesVerticalsFromVerticalMapper()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            _mockVerticalMapper.Received().Map(_gaugingSummaryItem, pointVelocityDischarge.Channel);
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsMeanObservationDurationAsNull()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

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

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MeanObservationDuration, Is.EqualTo(expectedMeanDuration));
        }

        private void SetupVerticalMapperToReturn(IEnumerable<Vertical> verticals)
        {
            _mockVerticalMapper.Map(Arg.Any<GaugingSummaryItem>(), Arg.Any<Channel>())
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

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MeanObservationDuration, Is.EqualTo(null));
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsWidthToNull()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

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

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.Width, Is.EqualTo(expectedTotalWidth));
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsAscendingDisplayOrderToTrue()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.True);
        }

        [Test]
        public void Map_TaglinePositionForFirstVerticalIsSmallerThanThatOfLastVertical_SetsAscendingDisplayOrderToTrue()
        {
            var verticals = _fixture.CreateMany<Vertical>(2).ToList();
            verticals.First().TaglinePosition = 0;
            verticals.Last().TaglinePosition = 10;

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.True);
        }

        [Test]
        public void Map_TaglinePositionForFirstVerticalIsLargerThanThatOfLastVertical_SetsAscendingDisplayOrderToFalse()
        {
            var verticals = _fixture.CreateMany<Vertical>(2).ToList();
            verticals.First().TaglinePosition = 10;
            verticals.Last().TaglinePosition = 0;

            SetupVerticalMapperToReturn(verticals);

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.AscendingSegmentDisplayOrder, Is.False);
        }

        [Test]
        public void Map_EmptyVerticalsList_SetsMaximumSegmentDischargeToNull()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

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

            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            Assert.That(pointVelocityDischarge.MaximumSegmentDischarge, Is.EqualTo(expectedMaximumSegmentDischarge));
        }
    }
}
