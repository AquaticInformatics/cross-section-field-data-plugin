using System;
using System.Collections.Generic;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class DischargeActivityMapper : IDischargeActivityMapper
    {
        private readonly IPointVelocityMapper _pointVelocityMapper;

        public DischargeActivityMapper(IPointVelocityMapper pointVelocityMapper)
        {
            _pointVelocityMapper = pointVelocityMapper;
        }

        public DischargeActivity Map(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset)
        {
            var dischargeActivity = CreateDischargeActivity(gaugingSummary, locationTimeZoneOffset);

            dischargeActivity.DischargeSubActivities = new List<DischargeSubActivity>
            {
                CreatePointVelocitySubActivity(gaugingSummary, dischargeActivity)
            };

            return dischargeActivity;
        }

        private DischargeSubActivity CreatePointVelocitySubActivity(GaugingSummaryItem gaugingSummary,
            DischargeActivity dischargeActivity)
        {
            return _pointVelocityMapper.Map(gaugingSummary, dischargeActivity);
        }

        private DischargeActivity CreateDischargeActivity(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset)
        {
            var startTime = new DateTimeOffset(gaugingSummary.StartDate, locationTimeZoneOffset);
            var endTime = new DateTimeOffset(gaugingSummary.EndDate, locationTimeZoneOffset);

            return new DischargeActivity
            {
                StartTime = startTime,
                EndTime = endTime,
                MeasurementTime = DateTimeHelper.GetMeanTime(startTime, endTime),
                Party = gaugingSummary.ObserversName,
                Discharge = gaugingSummary.Flow.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                DischargeUnitId = ParametersAndMethodsConstants.DischargeUnitId,
                DischargeMethodCode = GetDischargeMonitoringMethodCode(gaugingSummary.FlowCalculationMethod),
                MeanGageHeight = gaugingSummary.MeanStage,
                GageHeightUnitId = ParametersAndMethodsConstants.GageHeightUnitId,
                GageHeightMethodCode = ParametersAndMethodsConstants.GageHeightMethodCode,
                MeasurementId = gaugingSummary.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                MeanIndexVelocity = gaugingSummary.UseIndexVelocity ? gaugingSummary.IndexVelocity : default(double?),
                VelocityUnitId = ParametersAndMethodsConstants.VelocityUnitId,
                ShowInDataCorrection = true,
                ShowInRatingDevelopment = true
            };
        }

        private static string GetDischargeMonitoringMethodCode(FlowCalculationMethod? gaugingMethod)
        {
            switch (gaugingMethod)
            {
                case FlowCalculationMethod.Mean:
                    return ParametersAndMethodsConstants.MeanSectionMonitoringMethod;
                case FlowCalculationMethod.Mid:
                    return ParametersAndMethodsConstants.MidSectionMonitoringMethod;
                default:
                    return ParametersAndMethodsConstants.DefaultMonitoringMethod;
            }
        }
    }
}
