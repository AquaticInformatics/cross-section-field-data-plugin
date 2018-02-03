using System;
using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.Validation;
using FileHelpers;
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
        public DateTimeOffset MeasurementStartDateTime;

        [FieldOrder(4), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldConverter(typeof(CsvDateTimeOffsetConverter))]
        public DateTimeOffset MeasurementEndDateTime;

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

        [FieldOrder(10), FieldTrim(TrimMode.Both), FieldOptional]
        public string ChannelName;

        [FieldOrder(11), FieldTrim(TrimMode.Both), FieldOptional]
        public double? ChannelWidth;
        
        [FieldOrder(12), FieldTrim(TrimMode.Both), FieldOptional]
        public string WidthUnits;

        [FieldOrder(13), FieldTrim(TrimMode.Both), FieldOptional]
        public double? ChannelArea;

        [FieldOrder(14), FieldTrim(TrimMode.Both), FieldOptional]
        public string AreaUnits;

        [FieldOrder(15), FieldTrim(TrimMode.Both), FieldOptional]
        public double? ChannelVelocity;

        [FieldOrder(16), FieldTrim(TrimMode.Both), FieldOptional]
        public string VelocityUnits;

        [FieldOrder(17), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Party;

        [FieldOrder(18), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Comments;

        [FieldOrder(19), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading1Parameter;
        [FieldOrder(20), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading1Units;
        [FieldOrder(21), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Reading1Value;

        [FieldOrder(22), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading2Parameter;
        [FieldOrder(23), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading2Units;
        [FieldOrder(24), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Reading2Value;

        [FieldOrder(25), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading3Parameter;
        [FieldOrder(26), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading3Units;
        [FieldOrder(27), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Reading3Value;

        [FieldOrder(28), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading4Parameter;
        [FieldOrder(29), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading4Units;
        [FieldOrder(30), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Reading4Value;

        [FieldOrder(31), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading5Parameter;
        [FieldOrder(32), FieldTrim(TrimMode.Both), FieldQuoted(QuoteMode.OptionalForBoth, MultilineMode.NotAllow), FieldOptional]
        public string Reading5Units;
        [FieldOrder(33), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Reading5Value;

        public class Reading
        {
            public string Parameter { get; set; }
            public string Units { get; set; }
            public double Value { get; set; }
        }

        [FieldNotInFile]
        public List<Reading> Readings = new List<Reading>();

        public void Validate()
        {
            ValidationChecks.CannotBeNullOrEmpty(nameof(LocationIdentifier), LocationIdentifier);

            if (Discharge.HasValue)
            {
                ValidateDischargeValues();
            }
            else
            {
                ValidateEmptyDischarge();
            }

            ValidateAllReadings();

            ThrowIfNoDischargeOrReadings();

            ValidationChecks
                .MustBeAValidInterval(nameof(MeasurementStartDateTime), MeasurementStartDateTime, 
                                      nameof(MeasurementEndDateTime), MeasurementEndDateTime);
        }

        private void ValidateAllReadings()
        {
            AddValidReading(1, Reading1Parameter, Reading1Units, Reading1Value);
            AddValidReading(2, Reading2Parameter, Reading2Units, Reading2Value);
            AddValidReading(3, Reading3Parameter, Reading3Units, Reading3Value);
            AddValidReading(4, Reading4Parameter, Reading4Units, Reading4Value);
            AddValidReading(5, Reading5Parameter, Reading5Units, Reading5Value);
        }

        private void AddValidReading(int number, string parameter, string units, double? value)
        {
            if (!string.IsNullOrEmpty(parameter) && !string.IsNullOrEmpty(units) && value.HasValue)
            {
                Readings.Add(new Reading
                {
                    Parameter = parameter,
                    Units = units,
                    Value = value.Value
                });

                return;
            }

            var readingKey = $"Reading{number}";

            ThrowIfNotNull($"Reading{number}Value", value, readingKey);
            ThrowIfNotNullOrEmpty($"Reading{number}Parameter", parameter, readingKey);
            ThrowIfNotNullOrEmpty($"Reading{number}Units", units, readingKey);
        }

        private void ValidateDischargeValues()
        {
            ValidationChecks.CannotBeNull(nameof(StageAtStart), StageAtStart);
            ValidationChecks.CannotBeNull(nameof(StageAtEnd), StageAtEnd);
            ValidationChecks.CannotBeNullOrEmpty(nameof(StageUnits), StageUnits);
            ValidationChecks.CannotBeNull(nameof(Discharge), Discharge);
            ValidationChecks.CannotBeNullOrEmpty(nameof(DischargeUnits), DischargeUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(ChannelName), ChannelName);
            ValidationChecks.CannotBeNullOrEmpty(nameof(WidthUnits), WidthUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(AreaUnits), AreaUnits);
            ValidationChecks.CannotBeNullOrEmpty(nameof(VelocityUnits), VelocityUnits);
        }

        private void ValidateEmptyDischarge()
        {
            ThrowIfNotNull(nameof(StageAtStart), StageAtStart);
            ThrowIfNotNull(nameof(StageAtEnd), StageAtEnd);
            ThrowIfNotNullOrEmpty(nameof(StageUnits), StageUnits);
            ThrowIfNotNull(nameof(Discharge), Discharge);
            ThrowIfNotNullOrEmpty(nameof(DischargeUnits), DischargeUnits);
            ThrowIfNotNullOrEmpty(nameof(ChannelName), ChannelName);
            ThrowIfNotNullOrEmpty(nameof(WidthUnits), WidthUnits);
            ThrowIfNotNullOrEmpty(nameof(AreaUnits), AreaUnits);
            ThrowIfNotNullOrEmpty(nameof(VelocityUnits), VelocityUnits);
        }

        private void ThrowIfNotNull(string propertyName, double? value, string keyName = nameof(Discharge))
        {
            if (!value.HasValue) return;

            throw new ArgumentException($"{propertyName} must be empty when {keyName} is not set.");
        }

        private void ThrowIfNotNullOrEmpty(string propertyName, string value, string keyName = nameof(Discharge))
        {
            if (string.IsNullOrEmpty(value)) return;

            throw new ArgumentException($"{propertyName} must be empty when {keyName} is not set.");
        }

        private void ThrowIfNoDischargeOrReadings()
        {
            if (Discharge.HasValue || Readings.Any()) return;

            throw new ArgumentException($"Each row must contain at least one stage/discharge pair or at least one reading.");
        }
    }
}
