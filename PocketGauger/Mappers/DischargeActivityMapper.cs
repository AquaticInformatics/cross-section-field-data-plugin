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

            dischargeActivity.ChannelMeasurements.Add(CreatePointVelocitySubActivity(gaugingSummary, dischargeActivity));

            foreach (var gageHeightMeasurement in CreateGageHeightMeasurements(gaugingSummary, locationTimeZoneOffset))
            {
                dischargeActivity.GageHeightMeasurements.Add(gageHeightMeasurement);
            }

            return dischargeActivity;
        }

        private ChannelMeasurementBase CreatePointVelocitySubActivity(GaugingSummaryItem gaugingSummary, DischargeActivity dischargeActivity)
        {
            return _pointVelocityMapper.Map(gaugingSummary, dischargeActivity);
        }

        private static IEnumerable<GageHeightMeasurement> CreateGageHeightMeasurements(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset)
        {
            if (gaugingSummary.StartStage != null && gaugingSummary.EndStage != null)
            {
                yield return CreateGageHeightMeasurement(gaugingSummary.StartStage, gaugingSummary.StartDate, locationTimeZoneOffset);
                yield return CreateGageHeightMeasurement(gaugingSummary.EndStage, gaugingSummary.EndDate, locationTimeZoneOffset);
            }
            else if (gaugingSummary.MeanStage != null)
            {
                var measurementTime = DateTimeHelper.GetMean(gaugingSummary.StartDate, gaugingSummary.EndDate);

                yield return CreateGageHeightMeasurement(gaugingSummary.MeanStage, measurementTime, locationTimeZoneOffset);
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
                MeasurementId = GenerateMeasurementId(gaugingSummary),
                MeanIndexVelocity = GetMeanIndexVelocity(gaugingSummary)
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

        private static GageHeightMeasurement CreateGageHeightMeasurement(double? value, DateTime dateTime, TimeSpan timeZoneOffset)
        {
            return new GageHeightMeasurement(
                CreateMeasurement(value, ParametersAndMethodsConstants.DistanceUnitId),
                new DateTimeOffset(dateTime, timeZoneOffset));
        }

        private static Measurement CreateMeasurement(double? value, string unit)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Measurement(value.Value, unit);
        }
    }
}
