using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class PointVelocityMapper
    {
        private readonly IParseContext _context;
        private readonly IChannelInfo _channelInfo;
        private readonly GaugingSummaryItem _summaryItem;

        public PointVelocityMapper(IParseContext context, IChannelInfo channelInfo, GaugingSummaryItem summaryItem)
        {
            _context = context;
            _channelInfo = channelInfo;
            _summaryItem = summaryItem;
        }

        public DischargeSubActivity Map(DischargeActivity dischargeActivity)
        {
            return new PointVelocityDischarge
            {
                Area = _summaryItem.Area,
                AreaUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.AreaParameterId),
                ChannelMeasurement = CreateChannelMeasurement(dischargeActivity),
                DischargeMethod = MapPointVelocityMethod(dischargeActivity.DischargeMethod),
                MeasurementConditions = MeasurementCondition.OpenWater,
                StartPoint = MapStartPoint(_summaryItem.StartBank),
                TaglinePointUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                DistanceToMeterUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId),
                VelocityAverage = _summaryItem.MeanVelocity,
                VelocityAverageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
                VelocityObservationMethod = CalculateVelocityObservationMethod(_summaryItem),
                MeanObservationDuration = null, //TODO: AQ-19204 determine from verticals
                Width = null, //TODO: AQ-19204 Calculate from sum of all segment widths.
                WidthUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.WidthParameterId),
                AscendingSegmentDisplayOrder = true, //TODO: AQ-19204 determine from verticals
                MaximumSegmentDischarge = null, //TODO: AQ-19204 determine from verticals
                Verticals = new List<Vertical>() //TODO: AQ-19204 Create verticals
            };
        }

        private DischargeChannelMeasurement CreateChannelMeasurement(DischargeActivity dischargeActivity)
        {
            var meterSuspensionAndDeploymentMethod = MapMeterSuspensionAndDeploymentMethod();

            return new DischargeChannelMeasurement
            {
                StartTime = dischargeActivity.StartTime,
                EndTime = dischargeActivity.EndTime,
                Discharge = _summaryItem.Flow,
                Channel = _channelInfo,
                Comments = _summaryItem.Comments,
                Party = _summaryItem.ObserversName,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                MonitoringMethod = dischargeActivity.DischargeMethod,
                MeterSuspension = meterSuspensionAndDeploymentMethod.Key,
                DeploymentMethod = meterSuspensionAndDeploymentMethod.Value,
                DistanceToGageUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.DistanceToGageParameterId)
            };
        }

        private KeyValuePair<MeterSuspensionType, DeploymentMethodType> MapMeterSuspensionAndDeploymentMethod()
        {
            switch (_summaryItem.GaugingMethod)
            {
                case GaugingMethod.BridgeWithRods:
                    return CreateMeterSuspensionWithBridgeDeployment(MeterSuspensionType.RoundRod);
                case GaugingMethod.BridgeWithWinch:
                    return CreateMeterSuspensionWithBridgeDeployment(MeterSuspensionType.PackReel);
                case GaugingMethod.BridgeWithHandlines:
                    return CreateMeterSuspensionWithBridgeDeployment(MeterSuspensionType.Handline);
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

        private KeyValuePair<MeterSuspensionType, DeploymentMethodType> CreateMeterSuspensionWithBridgeDeployment(
            MeterSuspensionType meterSuspensionType)
        {
            var bridgeDeploymentMethod = DetermineBridgeDeploymentMethod();

            return CreateMeterSuspensionAndDeploymentPair(meterSuspensionType, bridgeDeploymentMethod);
        }

        private DeploymentMethodType DetermineBridgeDeploymentMethod()
        {
            var startPoint = MapStartPoint(_summaryItem.StartBank);

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
