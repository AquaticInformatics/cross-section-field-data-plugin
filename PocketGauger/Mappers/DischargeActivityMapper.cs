using System;
using System.Collections.Generic;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
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
            var surveyPeriod = new DateTimeInterval(startTime, endTime);
            var discharge = CreateMeasurement(gaugingSummary.Flow, ParametersAndMethodsConstants.DischargeUnitId);


            return new DischargeActivity(surveyPeriod, discharge)
            {
                Party = gaugingSummary.ObserversName,
                DischargeMethodCode = GetDischargeMonitoringMethodCode(gaugingSummary.FlowCalculationMethod),
                MeanGageHeight = gaugingSummary.MeanStage,
                GageHeightUnitId = ParametersAndMethodsConstants.GageHeightUnitId,
                GageHeightMethodCode = ParametersAndMethodsConstants.GageHeightMethodCode,
                MeasurementId = gaugingSummary.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                MeanIndexVelocity = gaugingSummary.UseIndexVelocity
                    ? CreateMeasurement(gaugingSummary.IndexVelocity, ParametersAndMethodsConstants.VelocityUnitId)
                    : null,
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

        private static Measurement CreateMeasurement(double? value, string unit)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Measurement(value.Value, unit);
        }
    }
}
