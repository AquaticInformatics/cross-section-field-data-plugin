using System;
using FieldDataPluginFramework;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class GaugingSummaryProcessor
    {
        private readonly IFieldDataResultsAppender _fieldDataResultsAppender;
        private readonly ILog _logger;

        public GaugingSummaryProcessor(IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            _fieldDataResultsAppender = fieldDataResultsAppender;
            _logger = logger;
        }

        public void ProcessGaugingSummary(GaugingSummary gaugingSummary)
        {
            try
            {
                var dischargeActivityMapper = CreateDischargeActivityMapper();

                foreach (var gaugingSummaryItem in gaugingSummary.GaugingSummaryItems)
                {
                    var locationIdentifier = gaugingSummaryItem.SiteId;
                    var location = _fieldDataResultsAppender.GetLocationByIdentifier(locationIdentifier);

                    var dischargeActivity = dischargeActivityMapper.Map(gaugingSummaryItem, location.UtcOffset);

                    var fieldVisit = CreateFieldVisit(dischargeActivity);
                    var fieldVisitInfo = _fieldDataResultsAppender.AddFieldVisit(location, fieldVisit);

                    _fieldDataResultsAppender.AddDischargeActivity(fieldVisitInfo, dischargeActivity);
                }

                _logger.Info(FormattableString.Invariant($"Processed gauging summary with {gaugingSummary.GaugingSummaryItems.Count} item(s)"));
            }
            catch (Exception e)
            {
                throw new PocketGaugerDataPersistenceException("Failed to persist pocket gauger data", e);
            }
        }

        private static DischargeActivityMapper CreateDischargeActivityMapper()
        {
            var meterCalibrationMapper = new MeterCalibrationMapper();
            var verticalMapper = new VerticalMapper(meterCalibrationMapper);
            var pointVelocityMapper = new PointVelocityMapper(verticalMapper);
            return new DischargeActivityMapper(pointVelocityMapper);
        }

        private static FieldVisitDetails CreateFieldVisit(DischargeActivity dischargeActivity)
        {
            return new FieldVisitDetails(dischargeActivity.MeasurementPeriod)
            {
                Party = dischargeActivity.Party
            };
        }
    }
}
