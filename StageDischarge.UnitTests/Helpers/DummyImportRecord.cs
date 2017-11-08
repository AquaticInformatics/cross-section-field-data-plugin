﻿using System;
using FieldDataPluginFramework.Validation;
using FileHelpers;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.Interfaces;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    [DelimitedRecord(CsvParserConstants.FieldDelimiter)]
    public class DummyImportRecord : ISelfValidator
    {
        [FieldOrder(1)] public string Id;
        [FieldOrder(2)] public string RecordString;
        [FieldOrder(3)] public DateTime? RecordDateTime;
        [FieldOrder(4)] public double? RecordOrdinal;
        [FieldOrder(5), FieldConverter(typeof(CsvDateTimeOffsetConverter))] public DateTimeOffset? RecordDateTimeOffset;
        [FieldOrder(6)] public bool? RecordBoolean;


        public void Validate()
        {
            ValidationChecks.CannotBeNullOrEmpty(nameof(Id), Id);
            ValidationChecks.CannotBeNullOrEmpty(nameof(RecordString), RecordString);
            ValidationChecks.CannotBeNull(nameof(RecordDateTime), RecordDateTime);
            ValidationChecks.CannotBeNull(nameof(RecordOrdinal), RecordOrdinal);
        }
    }
}
