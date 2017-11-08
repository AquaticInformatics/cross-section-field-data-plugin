using System;
using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class VerticalMapper : IVerticalMapper
    {
        private readonly IMeterCalibrationMapper _meterCalibrationMapper;

        public VerticalMapper(IMeterCalibrationMapper meterCalibrationMapper)
        {
            _meterCalibrationMapper = meterCalibrationMapper;
        }

        public List<Vertical> Map(GaugingSummaryItem gaugingSummaryItem, DeploymentMethodType deploymentMethod)
        {
            var verticals = new List<Vertical>();
            foreach (var panelItem in gaugingSummaryItem.PanelItems)
            {
                var vertical = CreateVertical(panelItem);
                vertical.Segment = CreateSegment(panelItem);
                vertical.VelocityObservation = CreateVelocityObservation(
                    deploymentMethod, panelItem, gaugingSummaryItem.MeterDetailsItem);

                verticals.Add(vertical);
            }

            SetVerticalTypeForFirstAndLastVertical(verticals);
            SetTotalDischargePortion(verticals);

            return verticals;
        }

        private static Vertical CreateVertical(PanelItem panelItem)
        {
            return new Vertical
            {
                SequenceNumber = panelItem.VerticalNumber.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                VerticalType = VerticalType.MidRiver,
                MeasurementConditionData = new OpenWaterData(),
                FlowDirection = FlowDirectionType.Normal,
                TaglinePosition = panelItem.Distance.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                SoundedDepth = panelItem.Depth.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                IsSoundedDepthEstimated = false,
                EffectiveDepth = panelItem.Depth.GetValueOrDefault() //TODO: AQ-19384 - Throw if this is null
            };
        }

        private static Segment CreateSegment(PanelItem panelItem)
        {
            return new Segment
            {
                Width = CalculateSegmentWidth(panelItem),
                Area = panelItem.Area.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                Velocity = panelItem.MeanVelocity.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                Discharge = panelItem.Flow.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                IsDischargeEstimated = false
            };
        }

        private static double CalculateSegmentWidth(PanelItem panelItem)
        {
            if (!panelItem.Depth.HasValue || IsEqual(panelItem.Depth.Value, 0))
                return 0;

            return Math.Abs(panelItem.Area.GetValueOrDefault()/panelItem.Depth.GetValueOrDefault());
        }

        private VelocityObservation CreateVelocityObservation(DeploymentMethodType deploymentMethod,
            PanelItem panelItem, MeterDetailsItem meterDetails)
        {
            var velocityObservation = new VelocityObservation
            {
                MeterCalibration = _meterCalibrationMapper.Map(meterDetails),
                VelocityObservationMethod = DetermineVelocityObservationMethodFromVerticals(panelItem.Verticals),
                DeploymentMethod = deploymentMethod,
                MeanVelocity = panelItem.MeanVelocity.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
            };

            foreach (var observation in panelItem.Verticals.Select(CreateVelocityDepthObservation))
            {
                velocityObservation.Observations.Add(observation);
            }

            return velocityObservation;
        }

        private static VelocityDepthObservation CreateVelocityDepthObservation(VerticalItem verticalItem)
        {
            return new VelocityDepthObservation
            {
                Depth = verticalItem.Depth.GetValueOrDefault() * verticalItem.SamplePosition.GetValueOrDefault(), //TODO: AQ-19384 - Throw if Depth is null
                RevolutionCount = (int?) verticalItem.Revs,
                ObservationInterval = verticalItem.ExposureTime,
                Velocity = verticalItem.Velocity.GetValueOrDefault() //TODO: AQ-19384 - Throw if this is null
            };
        }

        private static PointVelocityObservationType? DetermineVelocityObservationMethodFromVerticals(
            IReadOnlyCollection<VerticalItem> verticals)
        {
            switch (verticals.Count)
            {
                case 1:
                    return DetermineObservationMethodFromSamplePosition(verticals.First());
                case 2:
                    return PointVelocityObservationType.OneAtPointTwoAndPointEight;
                case 3:
                    return PointVelocityObservationType.OneAtPointTwoPointSixAndPointEight;
                case 5:
                    return PointVelocityObservationType.FivePoint;
                case 6:
                    return PointVelocityObservationType.SixPoint;
                case 11:
                    return PointVelocityObservationType.ElevenPoint;
                default:
                    return null;
            }
        }

        private static PointVelocityObservationType DetermineObservationMethodFromSamplePosition(VerticalItem observation)
        {
            const double pointFiveDepth = 0.5;
            const double pointSixDepth = 0.6;

            var depth = observation.SamplePosition;

            if (!depth.HasValue)
                return PointVelocityObservationType.Surface;

            if (IsEqual(depth.Value, pointFiveDepth))
                return PointVelocityObservationType.OneAtPointFive;

            if (IsEqual(depth.Value, pointSixDepth))
                return PointVelocityObservationType.OneAtPointSix;

            return PointVelocityObservationType.Surface;
        }

        private static bool IsEqual(double value, double otherValue)
        {
            return Math.Abs(value - otherValue) < double.Epsilon;
        }

        private static void SetVerticalTypeForFirstAndLastVertical(IReadOnlyCollection<Vertical> verticals)
        {
            if (!verticals.Any())
                return;

            verticals.First().VerticalType = VerticalType.StartEdgeNoWaterBefore;
            verticals.Last().VerticalType = VerticalType.EndEdgeNoWaterAfter;
        }

        private static void SetTotalDischargePortion(IReadOnlyCollection<Vertical> verticals)
        {
            var totalDischarge = verticals.Sum(v => v.Segment.Discharge);
            if (IsEqual(totalDischarge, 0)) return;

            foreach (var vertical in verticals)
            {
                vertical.Segment.TotalDischargePortion = vertical.Segment.Discharge/totalDischarge*100;
            }
        }
    }
}
