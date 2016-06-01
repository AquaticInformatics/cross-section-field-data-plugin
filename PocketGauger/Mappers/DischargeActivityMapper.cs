using System;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
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
            return new DischargeActivity
            {
                StartTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.StartDate, locationInfo),
                EndTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.EndDate, locationInfo),
                Party = gaugingSummaryItem.ObserversName,
                DischargeUnit = _context.DischargeParameter.DefaultUnit,
                GageHeightUnit = _context.GageHeightParameter.DefaultUnit,
                DischargeMethod = GetDischargeMonitoringMethod(gaugingSummaryItem.FlowCalculationMethod),
                GageHeightMethod = _context.GetDefaultMonitoringMethod()
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
