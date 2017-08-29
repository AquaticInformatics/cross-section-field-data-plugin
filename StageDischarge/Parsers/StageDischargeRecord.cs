using System;
using FileHelpers;
using Server.BusinessInterfaces.FieldDataPluginCore.Validation;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.Interfaces;

namespace Server.Plugins.FieldVisit.StageDischarge.Parsers
{
    [DelimitedRecord(CsvParserConstants.FieldDelimiter)]
    public class StageDischargeRecord : ISelfValidator
    {
        [FieldOrder(1), FieldTrim(TrimMode.Both)]
        public string LocationIdentifier;

        [FieldOrder(2), FieldTrim(TrimMode.Both)]
        public string MeasurementId;

        [FieldOrder(3), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldConverter(typeof(CsvDateTimeOffsetConverter))]
        public DateTime MeasurementStartDateTime;

        [FieldOrder(4), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldConverter(typeof(CsvDateTimeOffsetConverter))]
        public DateTime MeasurementEndDateTime;

        [FieldOrder(5), FieldTrim(TrimMode.Both)]
        public double? StageAtStart;

        [FieldOrder(6), FieldTrim(TrimMode.Both)]
        public double? StageAtEnd;

        [FieldOrder(7), FieldTrim(TrimMode.Both)]
        public string StageUnits;

        [FieldOrder(8), FieldTrim(TrimMode.Both)]
        public double? Discharge;

        [FieldOrder(9), FieldTrim(TrimMode.Both)]
        public string DischargeUnits;

        [FieldOrder(10), FieldTrim(TrimMode.Both)]
        public string ChannelName;

        [FieldOrder(11), FieldTrim(TrimMode.Both)]
        public double? ChannelWidth;
        
        [FieldOrder(12), FieldTrim(TrimMode.Both)]
        public string WidthUnits;

        [FieldOrder(13), FieldTrim(TrimMode.Both)]
        public double ChannelArea;

        [FieldOrder(14), FieldTrim(TrimMode.Both)]
        public string AreaUnits;

        [FieldOrder(15), FieldTrim(TrimMode.Both)]
        public double? ChannelVelocity;

        [FieldOrder(16), FieldTrim(TrimMode.Both)]
        public string VelocityUnits;

        [FieldOrder(17), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
        public string Party;

        [FieldOrder(18), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
        public string Comments;

        public void Validate()
        {
            ValidationChecks.CannotBeNullOrEmpty(nameof(LocationIdentifier), LocationIdentifier);
            ValidationChecks.CannotBeNull(nameof(StageAtStart), StageAtStart);
            ValidationChecks.CannotBeNull(nameof(StageAtEnd), StageAtEnd);
            ValidationChecks.CannotBeNullOrEmpty(nameof(StageUnits), StageUnits);
            ValidationChecks.CannotBeNull(nameof(Discharge), Discharge);
            ValidationChecks.CannotBeNullOrEmpty(nameof(DischargeUnits), DischargeUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(ChannelName), ChannelName);
            ValidationChecks.CannotBeNull(nameof(ChannelWidth), ChannelWidth);
            ValidationChecks.CannotBeNullOrEmpty(nameof(WidthUnits), WidthUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(AreaUnits), AreaUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(VelocityUnits), VelocityUnits);

            ValidationChecks
                .MustBeAValidInterval(nameof(MeasurementStartDateTime), MeasurementStartDateTime, 
                                      nameof(MeasurementEndDateTime), MeasurementEndDateTime);
        }
    }
}
