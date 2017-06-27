using System;
using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.Verticals;
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
            var meterSuspensionAndDeploymentMethod = MapMeterSuspensionAndDeploymentMethod(summaryItem);
            var verticals = _verticalMapper.Map(summaryItem, meterSuspensionAndDeploymentMethod.DeploymentMethod);

            return new ManualGaugingDischargeSection
            {
                StartTime = dischargeActivity.MeasurementPeriod.Start,
                EndTime = dischargeActivity.MeasurementPeriod.End,
                Party = summaryItem.ObserversName,
                ChannelName = ParametersAndMethodsConstants.DefaultChannelName,

                Discharge = summaryItem.Flow.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                DischargeUnitId = ParametersAndMethodsConstants.DischargeUnitId,

                MeterSuspension = meterSuspensionAndDeploymentMethod.MeterSuspension,
                DeploymentMethod = meterSuspensionAndDeploymentMethod.DeploymentMethod,
                Comments = summaryItem.Comments,

                Area = summaryItem.Area,
                AreaUnitId = ParametersAndMethodsConstants.AreaUnitId,
                DischargeMethod = MapDischargeMethod(summaryItem.FlowCalculationMethod),
                StartPoint = MapStartPoint(summaryItem.StartBank),
                TaglinePointUnitId = ParametersAndMethodsConstants.DistanceUnitId,
                VelocityAverage = summaryItem.MeanVelocity,
                VelocityAverageUnitId = ParametersAndMethodsConstants.VelocityUnitId,
                VelocityObservationMethod = CalculateVelocityObservationMethod(summaryItem),
                MeanObservationDuration = CalculateMeanObservationDuration(verticals),
                Width = CalculateTotalWidth(verticals),
                WidthUnitId = ParametersAndMethodsConstants.DistanceUnitId,
                TaglinePolarity = MapTaglinePolarity(verticals),
                MaximumSegmentDischarge = CalculateMaximumSegmentDischarge(verticals),
                Verticals = verticals
            };
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

        private static PointVelocityObservationType CalculateVelocityObservationMethod(GaugingSummaryItem summaryItem)
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

        private static TaglinePolarityType MapTaglinePolarity(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return default(TaglinePolarityType);

            var firstVertical = verticals.First();
            var lastVertical = verticals.Last();

            return firstVertical.TaglinePosition <= lastVertical.TaglinePosition
                ? TaglinePolarityType.Increasing
                : TaglinePolarityType.Decreasing;
        }

        private static double? CalculateMeanObservationDuration(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return null;

            var observationsWithInterval = verticals
                .SelectMany(vertical => vertical.VelocityObservation.Observations)
                .Where(observation => observation.ObservationInterval.HasValue)
                .ToList();

            if (!observationsWithInterval.Any())
                return null;

            return observationsWithInterval.Average(observation => observation.ObservationInterval);
        }

        private static double? CalculateTotalWidth(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return null;

            return verticals.Sum(vertical => vertical.Segment.Width);
        }

        private static double? CalculateMaximumSegmentDischarge(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return null;

            return verticals.Max(vertical => vertical.Segment.TotalDischargePortion);
        }
    }
}
