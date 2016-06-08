using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Verticals;
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

        public List<Vertical> Map(GaugingSummaryItem gaugingSummaryItem, PointVelocityDischarge pointVelocityDischarge)
        {
            var verticals = new List<Vertical>();
            foreach (var panelItem in gaugingSummaryItem.PanelItems)
            {
                var vertical = CreateVertical(panelItem);
                vertical.Segment = CreateSegment(gaugingSummaryItem.PanelItems.ToList(), panelItem);
                vertical.VelocityObservation = CreateVelocityObservation(pointVelocityDischarge, panelItem, gaugingSummaryItem);

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
                SequenceNumber = panelItem.VerticalNumber,
                VerticalType = VerticalType.MidRiver,
                MeasurementConditionData = new OpenWaterData(),
                FlowDirection = FlowDirectionType.Normal,
                TaglinePosition = panelItem.Distance,
                SoundedDepth = panelItem.Depth,
                IsSoundedDepthEstimated = false,
                EffectiveDepth = panelItem.Depth
            };
        }

        private static Segment CreateSegment(IList<PanelItem> panelItems, PanelItem panelItem)
        {
            return new Segment
            {
                Width = CalculateSegmentWidth(panelItems, panelItem),
                Area = panelItem.Area,
                Velocity = panelItem.MeanVelocity,
                Discharge = panelItem.Flow,
                IsDischargeEstimated = false
            };
        }

        private static double CalculateSegmentWidth(IList<PanelItem> panelItems, PanelItem panelItem)
        {
            if (panelItems.First() == panelItem)
            {
                return panelItem.Distance;
            }

            var previousPanelItem = panelItems[panelItems.IndexOf(panelItem) - 1];
            return panelItem.Distance - previousPanelItem.Distance;
        }

        private VelocityObservation CreateVelocityObservation(PointVelocityDischarge pointVelocityDischarge,
            PanelItem panelItem, GaugingSummaryItem gaugingSummaryItem)
        {
            return new VelocityObservation
            {
                MeterCalibration = _meterCalibrationMapper.Map(gaugingSummaryItem.MeterDetailsItem),
                VelocityObservationMethod = pointVelocityDischarge.VelocityObservationMethod,
                DeploymentMethod = pointVelocityDischarge.ChannelMeasurement.DeploymentMethod,
                MeanVelocity = panelItem.MeanVelocity,
                Observations = CreateObservations(panelItem.Verticals.ToList())
            };
        }

        private static List<VelocityDepthObservation> CreateObservations(IEnumerable<VerticalItem> verticalItems)
        {
            return verticalItems.Select(CreateVelocityDepthObservation).ToList();
        }

        private static VelocityDepthObservation CreateVelocityDepthObservation(VerticalItem verticalItem)
        {
            return new VelocityDepthObservation
            {
                Depth = verticalItem.Depth,
                RevolutionCount = (int) verticalItem.Revs,
                ObservationInterval = verticalItem.ExposureTime,
                Velocity = verticalItem.Velocity,
                DepthMultiplier = 1,
                Weighting = 1
            };
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
            if (totalDischarge == 0) return;

            foreach (var vertical in verticals)
            {
                vertical.Segment.TotalDischargePortion = vertical.Segment.Discharge/totalDischarge*100;
            }
        }
    }
}
