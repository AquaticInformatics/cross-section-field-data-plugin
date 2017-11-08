using System;
using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class PointVelocityMapper : IPointVelocityMapper
    {
        private class MeterSuspensionAndDeploymentPair
        {
            public MeterSuspensionType MeterSuspension { get; }
            public DeploymentMethodType DeploymentMethod { get; }

            public MeterSuspensionAndDeploymentPair(MeterSuspensionType meterSuspension, DeploymentMethodType deploymentMethod)
            {
                MeterSuspension = meterSuspension;
                DeploymentMethod = deploymentMethod;
            }
        }

        private readonly IVerticalMapper _verticalMapper;

        public PointVelocityMapper(IVerticalMapper verticalMapper)
        {
            _verticalMapper = verticalMapper;
        }

        public ManualGaugingDischargeSection Map(GaugingSummaryItem summaryItem, DischargeActivity dischargeActivity)
        {
            var dischargeSection = CreateDischargeSectionWithRawValuesFromSummaryItem(dischargeActivity.MeasurementPeriod, summaryItem);

            UpdateDischargeSectionWithDerivedValues(dischargeSection, summaryItem);
            UpdateDischargeSectionWithVerticals(dischargeSection, summaryItem);

            return dischargeSection;
        }

        private static ManualGaugingDischargeSection CreateDischargeSectionWithRawValuesFromSummaryItem(DateTimeInterval measurementPeriod, GaugingSummaryItem summaryItem)
        {
            if (!summaryItem.Area.HasValue)
                throw new ArgumentNullException(nameof(summaryItem.Area));
            if (!summaryItem.MeanVelocity.HasValue)
                throw new ArgumentNullException(nameof(summaryItem.MeanVelocity));

            var factory = new ManualGaugingDischargeSectionFactory(ParametersAndMethodsHelper.UnitSystem)
            {
                DefaultChannelName = ParametersAndMethodsHelper.DefaultChannelName
            };

            var dischargeSection = factory.CreateManualGaugingDischargeSection(measurementPeriod, summaryItem.Flow.AsDischargeMeasurement().Value);
            dischargeSection.Party = summaryItem.ObserversName;
            dischargeSection.Comments = summaryItem.Comments;
            dischargeSection.AreaValue = summaryItem.Area.Value;
            dischargeSection.VelocityAverageValue = summaryItem.MeanVelocity.Value;

            return dischargeSection;
        }

        private static void UpdateDischargeSectionWithDerivedValues(ManualGaugingDischargeSection dischargeSection, GaugingSummaryItem summaryItem)
        {
            dischargeSection.DischargeMethod = MapDischargeMethod(summaryItem.FlowCalculationMethod);
            dischargeSection.StartPoint = MapStartPoint(summaryItem.StartBank);
            dischargeSection.VelocityObservationMethod = DetermineVelocityObservationMethod(summaryItem);

            var meterSuspensionAndDeploymentMethod = MapMeterSuspensionAndDeploymentMethod(summaryItem);
            dischargeSection.MeterSuspension = meterSuspensionAndDeploymentMethod.MeterSuspension;
            dischargeSection.DeploymentMethod = meterSuspensionAndDeploymentMethod.DeploymentMethod;
        }

        private void UpdateDischargeSectionWithVerticals(ManualGaugingDischargeSection dischargeSection, GaugingSummaryItem summaryItem)
        {
            var verticals = _verticalMapper.Map(summaryItem, dischargeSection.DeploymentMethod);

            foreach (var vertical in verticals)
            {
                dischargeSection.Verticals.Add(vertical);
            }

            dischargeSection.WidthValue = CalculateTotalWidth(verticals);
        }

        private static MeterSuspensionAndDeploymentPair MapMeterSuspensionAndDeploymentMethod(GaugingSummaryItem summaryItem)
        {
            switch (summaryItem.GaugingMethod)
            {
                case GaugingMethod.BridgeWithRods:
                    return CreateMeterSuspensionWithBridgeDeployment(summaryItem, MeterSuspensionType.RoundRod);
                case GaugingMethod.BridgeWithWinch:
                    return CreateMeterSuspensionWithBridgeDeployment(summaryItem, MeterSuspensionType.PackReel);
                case GaugingMethod.BridgeWithHandlines:
                    return CreateMeterSuspensionWithBridgeDeployment(summaryItem, MeterSuspensionType.Handline);
                case GaugingMethod.BoatWithRods:
                    return CreateMeterSuspensionWithBoatDeployment(MeterSuspensionType.RoundRod);
                case GaugingMethod.BoatWithWinch:
                    return CreateMeterSuspensionWithBoatDeployment(MeterSuspensionType.PackReel);
                case GaugingMethod.BoatWithHandlines:
                    return CreateMeterSuspensionWithBoatDeployment(MeterSuspensionType.Handline);
                case GaugingMethod.Ice:
                    return new MeterSuspensionAndDeploymentPair(MeterSuspensionType.IceSurfaceMount, DeploymentMethodType.Ice);
                case GaugingMethod.Waded:
                    return new MeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Wading);
                case GaugingMethod.Cableway:
                    return new MeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Cableway);
                default:
                    return new MeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Unspecified);
            }
        }

        private static MeterSuspensionAndDeploymentPair CreateMeterSuspensionWithBridgeDeployment(
           GaugingSummaryItem summaryItem, MeterSuspensionType meterSuspensionType)
        {
            var bridgeDeploymentMethod = DetermineBridgeDeploymentMethod(summaryItem);

            return new MeterSuspensionAndDeploymentPair(meterSuspensionType, bridgeDeploymentMethod);
        }

        private static DeploymentMethodType DetermineBridgeDeploymentMethod(GaugingSummaryItem summaryItem)
        {
            var startPoint = MapNullableStartPoint(summaryItem.StartBank);

            switch (startPoint)
            {
                case StartPointType.LeftEdgeOfWater:
                    return DeploymentMethodType.BridgeDownstreamSide;
                case StartPointType.RightEdgeOfWater:
                    return DeploymentMethodType.BridgeUpstreamSide;
                case null:
                    return DeploymentMethodType.Unspecified;
                default:
                    return DeploymentMethodType.Unspecified;
            }
        }

        private static MeterSuspensionAndDeploymentPair CreateMeterSuspensionWithBoatDeployment(
            MeterSuspensionType meterSuspensionType)
        {
            return new MeterSuspensionAndDeploymentPair(meterSuspensionType, DeploymentMethodType.Boat);
        }

        private static PointVelocityObservationType DetermineVelocityObservationMethod(GaugingSummaryItem summaryItem)
        {
            if (summaryItem.SampleAt2 && summaryItem.SampleAt4 && summaryItem.SampleAt5 && summaryItem.SampleAt6 && summaryItem.SampleAt8 && summaryItem.SampleAtSurface && summaryItem.SampleAtBed)
                return PointVelocityObservationType.Unknown;

            if (summaryItem.SampleAt2 && summaryItem.SampleAt4 && summaryItem.SampleAt5 && summaryItem.SampleAt6 && summaryItem.SampleAt8 && (summaryItem.SampleAtSurface ^ summaryItem.SampleAtBed))
                return PointVelocityObservationType.SixPoint;

            if (summaryItem.SampleAt2 && summaryItem.SampleAt4 && summaryItem.SampleAt5 && summaryItem.SampleAt6 && summaryItem.SampleAt8)
                return PointVelocityObservationType.FivePoint;

            if (summaryItem.SampleAt2 && summaryItem.SampleAt6 && summaryItem.SampleAt8)
                return PointVelocityObservationType.OneAtPointTwoPointSixAndPointEight;

            if (summaryItem.SampleAt2 && summaryItem.SampleAt8)
                return PointVelocityObservationType.OneAtPointTwoAndPointEight;

            if (summaryItem.SampleAt6)
                return PointVelocityObservationType.OneAtPointSix;

            if (summaryItem.SampleAt5)
                return PointVelocityObservationType.OneAtPointFive;

            if (summaryItem.SampleAtSurface)
                return PointVelocityObservationType.Surface;

            return PointVelocityObservationType.Unknown;
        }

        private static StartPointType MapStartPoint(BankSide? startBank)
        {
            switch (startBank)
            {
                case null:
                    return default(StartPointType);
                case BankSide.Left:
                    return StartPointType.LeftEdgeOfWater;
                case BankSide.Right:
                    return StartPointType.RightEdgeOfWater;
                default:
                    throw new ArgumentException("Invalid start bank value", nameof(startBank));
            }
        }

        private static StartPointType? MapNullableStartPoint(BankSide? startBank)
        {
            return startBank.HasValue ? MapStartPoint(startBank) : default(StartPointType?);
        }

        private static DischargeMethodType MapDischargeMethod(FlowCalculationMethod? gaugingMethod)
        {
            switch (gaugingMethod)
            {
                case null:
                    return default(DischargeMethodType);
                case FlowCalculationMethod.Mean:
                    return DischargeMethodType.MeanSection;
                case FlowCalculationMethod.Mid:
                    return DischargeMethodType.MidSection;
                default:
                    throw new ArgumentException("Invalid flow calculation value", nameof(gaugingMethod));
            }
        }

        private static double? CalculateTotalWidth(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return null;

            return verticals.Sum(vertical => vertical.Segment.Width);
        }
    }
}
