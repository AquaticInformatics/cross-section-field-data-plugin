using System;
using FileHelpers;
using Server.BusinessInterfaces.FieldDataPluginCore.Validation;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.Parsers
{
    [DelimitedRecord(CsvParserConstants.FieldDelimiter)]
    public class StageDischargeRecord : ISelfValidator
    {
        [FieldOrder(1)]
        public string LocationIdentifier;

        [FieldOrder(2)]
        public string MeasurementId;

        [FieldOrder(3)]
        public DateTime MeasurementStartDateTime;

        [FieldOrder(4)]
        public DateTime MeasurementEndDateTime;

        [FieldOrder(5)]
        public double? StageAtStart;

        [FieldOrder(6)]
        public double? StageAtEnd;

        [FieldOrder(7)]
        public string StageUnits;

        [FieldOrder(8)]
        public double? Discharge;

        [FieldOrder(9)]
        public string DischargeUnits;

        [FieldOrder(10)]
        public string ChannelName;

        [FieldOrder(11)]
        public double? ChannelWidth;
        
        [FieldOrder(12)]
        public string WidthUnits;

        [FieldOrder(13)]
        public double ChannelArea;

        [FieldOrder(14)]
        public string AreaUnits;

        [FieldOrder(15)]
        public double? ChannelVelocity;

        [FieldOrder(16)]
        public string VelocityUnits;

        [FieldOrder(17)]
        public string Party;

        [FieldOrder(18)]
        public string Comments;

        public void Validate()
        {
            // be very careful that these rules match those specified in the AC of the work item

            // cannot be null
            ValidationChecks.CannotBeNullOrEmpty(nameof(LocationIdentifier), LocationIdentifier);
            ValidationChecks.CannotBeNull(nameof(StageAtStart), StageAtStart);
            ValidationChecks.CannotBeNull(nameof(StageAtEnd), StageAtEnd);
            ValidationChecks.CannotBeNullOrEmpty(nameof(StageUnits), StageUnits);
            ValidationChecks.CannotBeNull(nameof(Discharge), Discharge);
            ValidationChecks.CannotBeNullOrEmpty(nameof(DischargeUnits), DischargeUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(VelocityUnits), VelocityUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(WidthUnits), WidthUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(AreaUnits), AreaUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(ChannelName), ChannelName);
            ValidationChecks.CannotBeNull(nameof(ChannelWidth), ChannelWidth);
            ValidationChecks.CannotBeNullOrEmpty(nameof(VelocityUnits), VelocityUnits);

            // conditional relationships
            ValidationChecks
                .MustBeAValidInterval(nameof(MeasurementStartDateTime), MeasurementStartDateTime, 
                                      nameof(MeasurementEndDateTime), MeasurementEndDateTime);

            throw new NotImplementedException();
        }
    }
}
