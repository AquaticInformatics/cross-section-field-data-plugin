using System;
using System.Collections.Generic;
using System.Globalization;
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
    [TestFixture]
    public class DischargeActivityMapperTests
    {
        private IFixture _fixture;
        private IParseContext _context;
        private ILocationInfo _locationInfo;

        private DischargeActivityMapper _dischargeActivityMapper;
        private IVerticalMapper _verticalMapper;
        private GaugingSummaryItem _gaugingSummaryItem;

        private const int LocationUtcOffset = 3;

        [TestFixtureSetUp]
        public void SetupForAllTests()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            CollectionRegistrar.Register(_fixture);

            _context = new ParseContextTestHelper().CreateMockParseContext();

            SetupMockLocationInfo();
            SetupMockVerticalMapper();

            _dischargeActivityMapper = new DischargeActivityMapper(_context, _verticalMapper);
        }

        [SetUp]
        public void SetupForEachTest()
        {
            _gaugingSummaryItem = _fixture.Create<GaugingSummaryItem>();
        }

        private void SetupMockLocationInfo()
        {
            _locationInfo = Substitute.For<ILocationInfo>();

            var channel = Substitute.For<IChannelInfo>();
            _locationInfo.Channels.ReturnsForAnyArgs(new List<IChannelInfo> { channel });
            _locationInfo.UtcOffsetHours.ReturnsForAnyArgs(LocationUtcOffset);
        }

        private void SetupMockVerticalMapper()
        {
            _verticalMapper = Substitute.For<IVerticalMapper>();
            _verticalMapper.Map(null, null).ReturnsForAnyArgs(new List<Vertical>());
        }

        [Test]
        public void Map_GaugingSummaryStartDate_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.StartTime);
        }

        private static void AssertDateTimeOffsetIsNotDefault(DateTimeOffset dateTimeOffset)
        {
            Assert.That(dateTimeOffset.Offset, Is.EqualTo(TimeSpan.FromHours(LocationUtcOffset)));
            Assert.That(dateTimeOffset, Is.Not.EqualTo(default(DateTimeOffset)));
        }

        [Test]
        public void Map_GaugingSummaryEndDate_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.EndTime);
        }

        [Test]
        public void Map_DischargeActivityMeasurementTime_IsMappedDateTimeOffsetWithLocationUtcOffset()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            AssertDateTimeOffsetIsNotDefault(dischargeActivity.MeasurementTime);
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsMean_SetMonitoringMethodAsMeanSection()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = FlowCalculationMethod.Mean.ToString();

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            Assert.That(dischargeActivity.DischargeMethod.MethodCode, Is.EqualTo(ParametersAndMethodsConstants.MeanSectionMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsMid_SetMonitoringMethodAsMidSection()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = FlowCalculationMethod.Mid.ToString();

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            Assert.That(dischargeActivity.DischargeMethod.MethodCode, Is.EqualTo(ParametersAndMethodsConstants.MidSectionMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryFlowCalculationMethodIsUnknown_SetMonitoringMethodToDefault()
        {
            _gaugingSummaryItem.FlowCalculationMethodProxy = _fixture.Create<string>();

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            Assert.That(dischargeActivity.DischargeMethod.MethodCode, Is.EqualTo(ParametersAndMethodsConstants.DefaultMonitoringMethod));
        }

        [Test]
        public void Map_GaugingSummaryUseIndexVelocityFlagSet_SetsMeanIndexVelocity()
        {
            _gaugingSummaryItem.UseIndexVelocityProxy = bool.TrueString;

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            Assert.That(dischargeActivity.MeanIndexVelocity, Is.EqualTo(_gaugingSummaryItem.IndexVelocity));
        }

        [Test]
        public void Map_GaugingSummaryUseIndexVelocityFlagNotSet_SetsMeanIndexVelocityToNull()
        {
            _gaugingSummaryItem.UseIndexVelocityProxy = bool.FalseString;

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            Assert.That(dischargeActivity.MeanIndexVelocity, Is.EqualTo(null));
        }

        [Test]
        public void Map_GaugingSummaryItem_IsMappedToExpectedDischargeActivity()
        {
            var expectedDischargeActivity = CreateExpectedDischargeActivity();

            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

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
                Discharge = _gaugingSummaryItem.Flow,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                MeanGageHeight = _gaugingSummaryItem.MeanStage,
                GageHeightUnit = _context.GageHeightParameter.DefaultUnit,
                GageHeightMethod = _context.GetDefaultMonitoringMethod(),
                MeasurementId = _gaugingSummaryItem.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                VelocityUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
                ShowInDataCorrection = true,
                ShowInRatingDevelopment = true
            };
        }

        [Test]
        public void Map_RetrievesVerticalsFromVerticalMapper()
        {
            var dischargeActivity = _dischargeActivityMapper.Map(_locationInfo, _gaugingSummaryItem);

            var expectedChannelMeasurement =
                dischargeActivity.DischargeSubActivities.Single().ChannelMeasurement;
            _verticalMapper.Received().Map(_gaugingSummaryItem, expectedChannelMeasurement);
        }
    }
}
