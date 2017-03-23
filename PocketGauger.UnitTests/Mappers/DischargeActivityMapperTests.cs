using System;
using System.Globalization;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    [TestFixture]
    public class DischargeActivityMapperTests
    {
        private IFixture _fixture;
        private IParseContext _context;
        private IPointVelocityMapper _mockPointVelocityMapper;
        private GaugingSummaryItem _gaugingSummaryItem;

        private IDischargeActivityMapper _dischargeActivityMapper;

        private static TimeSpan LocationUtcOffset { get; } = TimeSpan.FromHours(3);

        [TestFixtureSetUp]
        public void SetupForAllTests()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            CollectionRegistrar.Register(_fixture);

            _context = new ParseContextTestHelper().CreateMockParseContext();

            SetupMockPointVelocityMapper();

            _dischargeActivityMapper = new DischargeActivityMapper(_mockPointVelocityMapper);
        }

        [SetUp]
        public void SetupForEachTest()
        {
            _gaugingSummaryItem = _fixture.Create<GaugingSummaryItem>();
        }

        private void SetupMockPointVelocityMapper()
        {
            _mockPointVelocityMapper = Substitute.For<IPointVelocityMapper>();
            _mockPointVelocityMapper.Map(null, null).ReturnsForAnyArgs(new PointVelocityDischarge());
        }

        [Test]
        public void Map_GaugingSummaryStartDate_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.StartTime);
        }

        private static void AssertDateTimeOffsetIsNotDefault(DateTimeOffset dateTimeOffset)
        {
            Assert.That(dateTimeOffset.Offset, Is.EqualTo(LocationUtcOffset));
            Assert.That(dateTimeOffset, Is.Not.EqualTo(default(DateTimeOffset)));
        }

        [Test]
        public void Map_GaugingSummaryEndDate_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.EndTime);
        }

        [Test]
        public void Map_DischargeActivityMeasurementTime_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.MeasurementTime);
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsMean_SetMonitoringMethodAsMeanSection()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = FlowCalculationMethod.Mean.ToString();

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            Assert.That(dischargeActivity.DischargeMethodCode, Is.EqualTo(ParametersAndMethodsConstants.MeanSectionMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsMid_SetMonitoringMethodAsMidSection()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = FlowCalculationMethod.Mid.ToString();

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            Assert.That(dischargeActivity.DischargeMethodCode, Is.EqualTo(ParametersAndMethodsConstants.MidSectionMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsUnknown_SetMonitoringMethodToDefault()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = _fixture.Create<string>();

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            Assert.That(dischargeActivity.DischargeMethodCode, Is.EqualTo(ParametersAndMethodsConstants.DefaultMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryUseIndexVelocityFlagSet_SetsMeanIndexVelocity()
        {
            _gaugingSummaryItem.UseIndexVelocityProxy = bool.TrueString;

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            Assert.That(dischargeActivity.MeanIndexVelocity, Is.EqualTo(_gaugingSummaryItem.IndexVelocity));
        }

        [Test]
        public void Map_GaugingSummaryUseIndexVelocityFlagNotSet_SetsMeanIndexVelocityToNull()
        {
            _gaugingSummaryItem.UseIndexVelocityProxy = bool.FalseString;

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            Assert.That(dischargeActivity.MeanIndexVelocity, Is.EqualTo(null));
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedDischargeActivity()
        {
            var expectedDischargeActivity = CreateExpectedDischargeActivity();

            var dischargeActivity = _dischargeActivityMapper.Map(_gaugingSummaryItem, LocationUtcOffset);

            dischargeActivity.ShouldBeEquivalentTo(expectedDischargeActivity, options => options
                .Excluding(activity => activity.StartTime)
                .Excluding(activity => activity.EndTime)
                .Excluding(activity => activity.MeasurementTime)
                .Excluding(activity => activity.DischargeMethod)
                .Excluding(activity => activity.MeanIndexVelocity)
                .Excluding(activity => activity.DischargeSubActivities));
        }

        private DischargeActivity CreateExpectedDischargeActivity()
        {
            return new DischargeActivity
            {
                Party = _gaugingSummaryItem.ObserversName,
                Discharge = _gaugingSummaryItem.Flow.GetValueOrDefault(),
                DischargeMethodCode = ParametersAndMethodsConstants.MidSectionMonitoringMethod,
                DischargeUnitId = ParametersAndMethodsConstants.DischargeUnitId,
                MeanGageHeight = _gaugingSummaryItem.MeanStage,
                GageHeightUnitId = ParametersAndMethodsConstants.DistanceUnitId,
                GageHeightMethodCode = ParametersAndMethodsConstants.GageHeightMethodCode,
                MeasurementId = _gaugingSummaryItem.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                VelocityUnitId = ParametersAndMethodsConstants.VelocityUnitId,
                ShowInDataCorrection = true,
                ShowInRatingDevelopment = true
            };
        }
    }
}
