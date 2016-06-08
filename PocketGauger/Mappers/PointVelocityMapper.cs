using System.Collections.Generic;
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

        public PointVelocityMapper(IParseContext context)
        {
            _context = context;
        }

        public PointVelocityDischarge Map(IChannelInfo channelInfo, GaugingSummaryItem summaryItem, DischargeActivity dischargeActivity)
        {
            var channelMeasurement = CreateChannelMeasurement(channelInfo, summaryItem, dischargeActivity);

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
                MeanObservationDuration = null, //TODO: AQ-19204 determine from verticals
                Width = null, //TODO: AQ-19204 Calculate from sum of all segment widths.
                WidthUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.WidthParameterId),
                AscendingSegmentDisplayOrder = true, //TODO: AQ-19204 determine from verticals
                MaximumSegmentDischarge = null, //TODO: AQ-19204 determine from verticals
                Verticals = new List<Vertical>() //TODO: AQ-19204 Create verticals
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
                Discharge = summaryItem.Flow,
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
    }
}
