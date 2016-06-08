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
    public class DischargeActivityMapper
    {
        private readonly IParseContext _context;
        private readonly IVerticalMapper _verticalMapper;

        public DischargeActivityMapper(IParseContext context, IVerticalMapper verticalMapper)
        {
            _context = context;
            _verticalMapper = verticalMapper;
        }

        public DischargeActivity Map(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummaryItem)
        {
            var dischargeActivity = CreateDischargeActivity(locationInfo, gaugingSummaryItem);

            var pointVelocityMapper = new PointVelocityMapper(_context, GetDefaultChannel(locationInfo), gaugingSummaryItem);
            dischargeActivity.DischargeSubActivities = new List<DischargeSubActivity>
            {
                CreatePointVelocitySubActivity(pointVelocityMapper, dischargeActivity, gaugingSummaryItem)
            };

            return dischargeActivity;
        }

        private DischargeSubActivity CreatePointVelocitySubActivity(PointVelocityMapper pointVelocityMapper,
            DischargeActivity dischargeActivity, GaugingSummaryItem gaugingSummaryItem)
        {
            var pointVelocitySubActivity = pointVelocityMapper.Map(dischargeActivity);
            pointVelocitySubActivity.Verticals = _verticalMapper.Map(gaugingSummaryItem, pointVelocitySubActivity.ChannelMeasurement).ToList();

            return pointVelocitySubActivity;
        }

        private static IChannelInfo GetDefaultChannel(ILocationInfo locationInfo)
        {
            return ChannelHelper.GetDefaultLocationChannel(locationInfo);
        }

        private DischargeActivity CreateDischargeActivity(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummaryItem)
        {
            var startTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.StartDate, locationInfo);
            var endTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.EndDate, locationInfo);

            return new DischargeActivity
            {
                StartTime = startTime,
                EndTime = endTime,
                MeasurementTime = DateTimeHelper.GetMeanTime(startTime, endTime),
                Party = gaugingSummaryItem.ObserversName,
                Discharge = gaugingSummaryItem.Flow,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                DischargeMethod = GetDischargeMonitoringMethod(gaugingSummaryItem.FlowCalculationMethod),
                MeanGageHeight = gaugingSummaryItem.MeanStage,
                GageHeightUnit = _context.GageHeightParameter.DefaultUnit,
                GageHeightMethod = _context.GetDefaultMonitoringMethod(),
                MeasurementId = gaugingSummaryItem.GaugingId.ToString(NumberFormatInfo.InvariantInfo),
                MeanIndexVelocity = gaugingSummaryItem.UseIndexVelocity ? gaugingSummaryItem.IndexVelocity : default(double?),
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
