using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private readonly IVerticalMapper _verticalMapper;
        private readonly IPointVelocityMapper _pointVelocityMapper;

        public DischargeActivityMapper(IParseContext context, IVerticalMapper verticalMapper, IPointVelocityMapper pointVelocityMapper)
        {
            _context = context;
            _verticalMapper = verticalMapper;
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
            var pointVelocitySubActivity = _pointVelocityMapper.Map(channel, gaugingSummary, dischargeActivity);
            pointVelocitySubActivity.Verticals = _verticalMapper.Map(gaugingSummary, pointVelocitySubActivity.ChannelMeasurement).ToList();

            return pointVelocitySubActivity;
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
                Discharge = gaugingSummary.Flow,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                DischargeMethod = GetDischargeMonitoringMethod(gaugingSummary.FlowCalculationMethod),
                MeanGageHeight = gaugingSummary.MeanStage,
                GageHeightUnit = _context.GageHeightParameter.DefaultUnit,
                GageHeightMethod = _context.GetDefaultMonitoringMethod(),
                MeasurementId = gaugingSummary.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                MeanIndexVelocity = gaugingSummary.UseIndexVelocity ? gaugingSummary.IndexVelocity : default(double?),
                VelocityUnit = _context.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId),
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
    }
}
