using System;
using System.Collections.Generic;
using System.Globalization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class DischargeActivityMapper
    {
        private readonly IParseContext _context;

        public DischargeActivityMapper(IParseContext context)
        {
            _context = context;
        }

        public DischargeActivity Map(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummaryItem)
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
