using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.Verticals;
using Server.BusinessInterfaces.FieldDataPluginCore.Units;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge.Mappers
{
    internal class DischargeActivityMapper
    {
        public DischargeActivity FromStageDischargeRecord(StageDischargeRecord record)
        {
            var discharge = new Measurement(record.Discharge.GetValueOrDefault(), record.DischargeUnits);
            var dischargeInterval = new DateTimeInterval(record.MeasurementStartDateTime, record.MeasurementEndDateTime);
            var activity = new DischargeActivity(dischargeInterval, discharge)
            {
                MeasurementId = record.MeasurementId,
                Comments = record.Comments,
                Party = record.Party
            };

            activity.GageHeightMeasurements.Add(new GageHeightMeasurement(new Measurement(record.StageAtStart.GetValueOrDefault(), record.StageUnits), record.MeasurementStartDateTime));
            activity.GageHeightMeasurements.Add(new GageHeightMeasurement(new Measurement(record.StageAtEnd.GetValueOrDefault(), record.StageUnits), record.MeasurementEndDateTime));

            var manualGaugingDischarge = CreateChannelMeasurementFromRecord(record, dischargeInterval, discharge);

            if (manualGaugingDischarge != null)
            {
                activity.ChannelMeasurements.Add(manualGaugingDischarge);
            }

            return activity;
        }

        private static ChannelMeasurementBase CreateChannelMeasurementFromRecord(StageDischargeRecord record, DateTimeInterval dischargeInterval, Measurement discharge)
        {
            // use this instead?
            ManualGaugingDischargeSection section 
                = new ManualGaugingDischargeSectionFactory(CreateUnitSystem(record))
                    {
                        DefaultChannelName = record.ChannelName
                    }
                    .CreateManualGaugingDischargeSection(dischargeInterval, discharge.Value);

            section.AreaValue = record.ChannelArea;
            section.AreaUnitId = record.AreaUnits;
            section.WidthValue = record.ChannelWidth;
            section.DistanceUnitId = record.WidthUnits;
            section.VelocityAverageValue = record.ChannelVelocity;
            section.VelocityUnitId = record.VelocityUnits;
            section.ChannelName = record.ChannelName;
            section.Party = record.Party;

            section.DischargeMethod = DischargeMethodType.MidSection;
            section.VelocityObservationMethod = PointVelocityObservationType.Unknown;
            section.DeploymentMethod = DeploymentMethodType.Unspecified;
            section.MeterSuspension = MeterSuspensionType.Unspecified;

            return section;
        }

        private static UnitSystem CreateUnitSystem(StageDischargeRecord record)
        {
            return new UnitSystem
            {
                DistanceUnitId = record.WidthUnits,
                AreaUnitId = record.AreaUnits,
                DischargeUnitId = record.DischargeUnits,
                VelocityUnitId = record.VelocityUnits
            };
        }
    }
}
