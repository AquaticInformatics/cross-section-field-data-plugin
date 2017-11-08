// ReSharper disable PossibleInvalidOperationException
using System;
using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.DataModel.Verticals;
using FieldDataPluginFramework.UnitTests.TestHelpers;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
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
            _mockVerticalMapper.Map(null, default(DeploymentMethodType)).ReturnsForAnyArgs(new List<Vertical>());
        }

        private void SetupAutoFixture()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            _fixture.AddFieldDataPluginFrameworkTestingExtensions();
            CollectionRegistrar.Register(_fixture);
        }

        [SetUp]
        public void SetupForEachTest()
        {
            _dischargeActivity = _fixture.Create<DischargeActivity>();
        }

        [TestCase(BankSide.Left, StartPointType.LeftEdgeOfWater)]
        [TestCase(BankSide.Right, StartPointType.RightEdgeOfWater)]
        [TestCase(null, default(StartPointType))]
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
        [TestCase(null, default(DischargeMethodType))]
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

            AssertDischargeSectionHasExpectedSuspensionAndDeploymentType(pointVelocityDischarge,
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        private static void AssertDischargeSectionHasExpectedSuspensionAndDeploymentType(ManualGaugingDischargeSection dischargeSection,
            MeterSuspensionType meterSuspensionType, DeploymentMethodType expectedMeterAndDeploymentType)
        {
            Assert.That(dischargeSection.MeterSuspension, Is.EqualTo(meterSuspensionType));
            Assert.That(dischargeSection.DeploymentMethod, Is.EqualTo(expectedMeterAndDeploymentType));
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

            var manualGaugingDischargeSection = MapToManualGaugingDischargeSection();

            AssertDischargeSectionHasExpectedSuspensionAndDeploymentType(manualGaugingDischargeSection,
                expectedMeterAndDeploymentType.Item1, expectedMeterAndDeploymentType.Item2);
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedPointVelocityDischarge()
        {
            var expectedDischargeActivity = CreateExpectedManualGaugingDischargeSection();

            var pointVelocityDischarge = MapToManualGaugingDischargeSection();

            pointVelocityDischarge.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.Verticals)
                .Excluding(activity => activity.DischargeMethod)
                .Excluding(activity => activity.VelocityObservationMethod)
                .Excluding(activity => activity.StartPoint)
                .Excluding(activity => activity.MeterSuspension)
                .Excluding(activity => activity.DeploymentMethod));
        }

        private ManualGaugingDischargeSection CreateExpectedManualGaugingDischargeSection()
        {
            var factory = new ManualGaugingDischargeSectionFactory(ParametersAndMethodsHelper.UnitSystem)
            {
                DefaultChannelName = ParametersAndMethodsHelper.DefaultChannelName
            };

            var dischargeSection = factory.CreateManualGaugingDischargeSection(_dischargeActivity.MeasurementPeriod, _gaugingSummaryItem.Flow.Value);
            dischargeSection.Party = _gaugingSummaryItem.ObserversName;
            dischargeSection.Comments = _gaugingSummaryItem.Comments;
            dischargeSection.AreaValue = _gaugingSummaryItem.Area.Value;
            dischargeSection.VelocityAverageValue = _gaugingSummaryItem.MeanVelocity.Value;

            return dischargeSection;
        }

        [Test]
        public void Map_RetrievesVerticalsFromVerticalMapper()
        {
            var pointVelocityDischarge = _mapper.Map(_gaugingSummaryItem, _dischargeActivity);

            _mockVerticalMapper.Received().Map(_gaugingSummaryItem, pointVelocityDischarge.DeploymentMethod);
        }

        private void SetupVerticalMapperToReturn(IEnumerable<Vertical> verticals)
        {
            _mockVerticalMapper.Map(Arg.Any<GaugingSummaryItem>(), Arg.Any<DeploymentMethodType>())
                .Returns(verticals.ToList());
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

            Assert.That(pointVelocityDischarge.Width.Value, Is.EqualTo(expectedTotalWidth));
        }
    }
}
