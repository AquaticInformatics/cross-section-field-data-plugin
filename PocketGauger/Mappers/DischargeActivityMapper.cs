using System;
using System.Collections.Generic;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
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

            dischargeActivity.ChannelMeasurements = new List<ChannelMeasurementBase>
            {
                CreatePointVelocitySubActivity(gaugingSummary, dischargeActivity)
            };

            dischargeActivity.GageHeightMeasurements = new List<GageHeightMeasurement>();
            dischargeActivity.GageHeightMeasurements.AddRange(CreateGageHeightMeasurements(gaugingSummary, locationTimeZoneOffset));

            return dischargeActivity;
        }

        private ChannelMeasurementBase CreatePointVelocitySubActivity(GaugingSummaryItem gaugingSummary,
            DischargeActivity dischargeActivity)
        {
            return _pointVelocityMapper.Map(gaugingSummary, dischargeActivity);
        }

        private static IEnumerable<GageHeightMeasurement> CreateGageHeightMeasurements(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset)
        {
            if (gaugingSummary.StartStage != null && gaugingSummary.EndStage != null)
            {
                yield return new GageHeightMeasurement
                {
                    MeasurementTime = new DateTimeOffset(gaugingSummary.StartDate, locationTimeZoneOffset),
                    GageHeight = CreateMeasurement(gaugingSummary.StartStage, ParametersAndMethodsConstants.GageHeightUnitId)
                };
                yield return new GageHeightMeasurement
                {
                    MeasurementTime = new DateTimeOffset(gaugingSummary.EndDate, locationTimeZoneOffset),
                    GageHeight = CreateMeasurement(gaugingSummary.EndStage, ParametersAndMethodsConstants.GageHeightUnitId)
                };
            }
            else if (gaugingSummary.MeanStage != null)
            {
                yield return new GageHeightMeasurement
                {
                    MeasurementTime = new DateTimeOffset((gaugingSummary.StartDate.Ticks + gaugingSummary.EndDate.Ticks) / 2, locationTimeZoneOffset),
                    GageHeight = CreateMeasurement(gaugingSummary.MeanStage, ParametersAndMethodsConstants.GageHeightUnitId)
                };
            }
        }

        private static DischargeActivity CreateDischargeActivity(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset)
        {
            var startTime = new DateTimeOffset(gaugingSummary.StartDate, locationTimeZoneOffset);
            var endTime = new DateTimeOffset(gaugingSummary.EndDate, locationTimeZoneOffset);
            var surveyPeriod = new DateTimeInterval(startTime, endTime);
            var discharge = CreateMeasurement(gaugingSummary.Flow, ParametersAndMethodsConstants.DischargeUnitId);

            return new DischargeActivity(surveyPeriod, discharge)
            {
                Party = gaugingSummary.ObserversName,
                DischargeMethodCode = GetDischargeMonitoringMethodCode(gaugingSummary.FlowCalculationMethod),
                MeasurementId = GenerateMeasurementId(gaugingSummary),
                MeanIndexVelocity = GetMeanIndexVelocity(gaugingSummary),
                ShowInDataCorrection = true,
                ShowInRatingDevelopment = true
            };
        }

        private static string GenerateMeasurementId(GaugingSummaryItem gaugingSummary)
        {
            return gaugingSummary.GaugingId.ToString(NumberFormatInfo.InvariantInfo);
        }

        private static Measurement GetMeanIndexVelocity(GaugingSummaryItem gaugingSummary)
        {
            return gaugingSummary.UseIndexVelocity
                ? CreateMeasurement(gaugingSummary.IndexVelocity, ParametersAndMethodsConstants.VelocityUnitId)
                : null;
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
