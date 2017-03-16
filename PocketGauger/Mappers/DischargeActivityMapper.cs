using System;
using System.Collections.Generic;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class DischargeActivityMapper : IDischargeActivityMapper
    {
        private readonly IParseContext _context;
        private readonly IPointVelocityMapper _pointVelocityMapper;

        public DischargeActivityMapper(IPointVelocityMapper pointVelocityMapper)
        {
            _pointVelocityMapper = pointVelocityMapper;
        }

        public DischargeActivityMapper(IParseContext context, IPointVelocityMapper pointVelocityMapper)
        {
            _context = context;
            _pointVelocityMapper = pointVelocityMapper;
        }

        public DischargeActivity Map(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummary)
        {
            var dischargeActivity = CreateDischargeActivity(locationInfo, gaugingSummary);
            var channel = GetDefaultChannel(locationInfo);

            dischargeActivity.DischargeSubActivities = new List<DischargeSubActivity>
            {
                CreatePointVelocitySubActivity(channel, gaugingSummary, dischargeActivity)
            };

            return dischargeActivity;
        }

        private static IChannelInfo GetDefaultChannel(ILocationInfo locationInfo)
        {
            return ChannelHelper.GetDefaultLocationChannel(locationInfo);
        }

        private DischargeSubActivity CreatePointVelocitySubActivity(IChannelInfo channel, GaugingSummaryItem gaugingSummary,
            DischargeActivity dischargeActivity)
        {
            return _pointVelocityMapper.Map(channel, gaugingSummary, dischargeActivity);
        }

        private DischargeActivity CreateDischargeActivity(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummary)
        {
            var startTime = CreateLocationBasedDateTimeOffset(gaugingSummary.StartDate, locationInfo);
            var endTime = CreateLocationBasedDateTimeOffset(gaugingSummary.EndDate, locationInfo);

            return new DischargeActivity
            {
                StartTime = startTime,
                EndTime = endTime,
                MeasurementTime = DateTimeHelper.GetMeanTime(startTime, endTime),
                Party = gaugingSummary.ObserversName,
                Discharge = gaugingSummary.Flow.GetValueOrDefault(), //TODO: AQ-19384 - Throw if this is null
                DischargeUnit = _context?.DischargeParameter.DefaultUnit,
                DischargeMethod = GetDischargeMonitoringMethod(gaugingSummary.FlowCalculationMethod),
                MeanGageHeight = gaugingSummary.MeanStage,
                GageHeightUnit = _context?.GageHeightParameter.DefaultUnit,
                GageHeightMethod = _context?.GetDefaultMonitoringMethod(),
                MeasurementId = gaugingSummary.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                MeanIndexVelocity = gaugingSummary.UseIndexVelocity ? gaugingSummary.IndexVelocity : default(double?),
                VelocityUnit = _context?.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
                ShowInDataCorrection = true,
                ShowInRatingDevelopment = true
            };
        }

        private static DateTimeOffset CreateLocationBasedDateTimeOffset(DateTime dateTime, ILocationInfo locationInfo)
        {
            return new DateTimeOffset(dateTime, TimeSpan.FromHours(locationInfo.UtcOffsetHours));
        }

        private IMonitoringMethod GetDischargeMonitoringMethod(FlowCalculationMethod? gaugingMethod)
        {
            switch (gaugingMethod)
            {
                case FlowCalculationMethod.Mean:
                    return _context.DischargeParameter.GetMonitoringMethod(ParametersAndMethodsConstants.MeanSectionMonitoringMethod);
                case FlowCalculationMethod.Mid:
                    return _context.DischargeParameter.GetMonitoringMethod(ParametersAndMethodsConstants.MidSectionMonitoringMethod);
                default:
                    return _context.GetDefaultMonitoringMethod();
            }
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
