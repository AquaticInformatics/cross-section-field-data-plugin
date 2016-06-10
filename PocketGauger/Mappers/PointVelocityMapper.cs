using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class PointVelocityMapper : IPointVelocityMapper
    {
        private readonly IParseContext _context;
        private readonly IVerticalMapper _verticalMapper;

        public PointVelocityMapper(IParseContext context, IVerticalMapper verticalMapper)
        {
            _context = context;
            _verticalMapper = verticalMapper;
        }

        public PointVelocityDischarge Map(IChannelInfo channelInfo, GaugingSummaryItem summaryItem, DischargeActivity dischargeActivity)
        {
            var channelMeasurement = CreateChannelMeasurement(channelInfo, summaryItem, dischargeActivity);
            var verticals = _verticalMapper.Map(summaryItem, channelMeasurement);

            return new PointVelocityDischarge
            {
                Area = summaryItem.Area,
                AreaUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.AreaParameterId),
                ChannelMeasurement = channelMeasurement,
                DischargeMethod = MapPointVelocityMethod(dischargeActivity.DischargeMethod),
                MeasurementConditions = MeasurementCondition.OpenWater,
                StartPoint = MapStartPoint(summaryItem.StartBank),
                TaglinePointUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                DistanceToMeterUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                VelocityAverage = summaryItem.MeanVelocity,
                VelocityAverageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
                VelocityObservationMethod = CalculateVelocityObservationMethod(summaryItem),
                MeanObservationDuration = CalculateMeanObservationDuration(verticals),
                Width = CalculateTotalWidth(verticals),
                WidthUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.WidthParameterId),
                AscendingSegmentDisplayOrder = IsAscendingDisplayOrder(verticals),
                MaximumSegmentDischarge = CalculateMaximumSegmentDischarge(verticals),
                Verticals = verticals
            };
        }

        private DischargeChannelMeasurement CreateChannelMeasurement(IChannelInfo channelInfo, GaugingSummaryItem summaryItem, 
            DischargeActivity dischargeActivity)
        {
            var meterSuspensionAndDeploymentMethod = MapMeterSuspensionAndDeploymentMethod(summaryItem);

            return new DischargeChannelMeasurement
            {
                StartTime = dischargeActivity.StartTime,
                EndTime = dischargeActivity.EndTime,
                Discharge = summaryItem.Flow.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                Channel = channelInfo,
                Comments = summaryItem.Comments,
                Party = summaryItem.ObserversName,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                MonitoringMethod = dischargeActivity.DischargeMethod,
                MeterSuspension = meterSuspensionAndDeploymentMethod.Key,
                DeploymentMethod = meterSuspensionAndDeploymentMethod.Value,
                DistanceToGageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId)
            };
        }

        private static KeyValuePair<MeterSuspensionType, DeploymentMethodType> MapMeterSuspensionAndDeploymentMethod(GaugingSummaryItem summaryItem)
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
                    return CreateMeterSuspensionAndDeploymentPair(MeterSuspensionType.IceSurfaceMount, DeploymentMethodType.Ice);
                case GaugingMethod.Waded:
                    return CreateMeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Wading);
                case GaugingMethod.Cableway:
                    return CreateMeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Cableway);
                default:
                    return CreateMeterSuspensionAndDeploymentPair(MeterSuspensionType.Unspecified, DeploymentMethodType.Unspecified);
            }
        }

        private static KeyValuePair<MeterSuspensionType, DeploymentMethodType> CreateMeterSuspensionAndDeploymentPair(
            MeterSuspensionType meterSuspensionType, DeploymentMethodType deploymentMethodType)
        {
            return new KeyValuePair<MeterSuspensionType, DeploymentMethodType>(meterSuspensionType,
                deploymentMethodType);
        }

        private static KeyValuePair<MeterSuspensionType, DeploymentMethodType> CreateMeterSuspensionWithBridgeDeployment(
            GaugingSummaryItem summaryItem, MeterSuspensionType meterSuspensionType)
        {
            var bridgeDeploymentMethod = DetermineBridgeDeploymentMethod(summaryItem);

            return CreateMeterSuspensionAndDeploymentPair(meterSuspensionType, bridgeDeploymentMethod);
        }

        private static DeploymentMethodType DetermineBridgeDeploymentMethod(GaugingSummaryItem summaryItem)
        {
            var startPoint = MapStartPoint(summaryItem.StartBank);

            switch (startPoint)
            {
                case StartPointType.LeftEdgeOfWater:
                    return DeploymentMethodType.BridgeDownstreamSide;
                case StartPointType.RightEdgeOfWater:
                    return DeploymentMethodType.BridgeUpstreamSide;
                default:
                    return DeploymentMethodType.Unspecified;
            }
        }

        private static KeyValuePair<MeterSuspensionType, DeploymentMethodType> CreateMeterSuspensionWithBoatDeployment(
            MeterSuspensionType meterSuspensionType)
        {
            return CreateMeterSuspensionAndDeploymentPair(meterSuspensionType, DeploymentMethodType.Boat);
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
                case BankSide.Left:
                    return StartPointType.LeftEdgeOfWater;
                case BankSide.Right:
                    return StartPointType.RightEdgeOfWater;
                default:
                    return StartPointType.Unspecified;
            }
        }

        private static PointVelocityMethodType MapPointVelocityMethod(IMonitoringMethod gaugingMethod)
        {
            switch (gaugingMethod.MethodCode)
            {
                case ParametersAndMethodsConstants.MeanSectionMonitoringMethod:
                    return PointVelocityMethodType.MeanSection;
                case ParametersAndMethodsConstants.MidSectionMonitoringMethod:
                    return PointVelocityMethodType.MidSection;
                default:
                    return PointVelocityMethodType.Unknown;
            }
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

        private static bool IsAscendingDisplayOrder(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return true;

            var firstVertical = verticals.First();
            var lastVertical = verticals.Last();

            return firstVertical.TaglinePosition < lastVertical.TaglinePosition;
        }

        private static double? CalculateMaximumSegmentDischarge(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return null;

            return verticals.Max(vertical => vertical.Segment.TotalDischargePortion);
        }
    }
}
