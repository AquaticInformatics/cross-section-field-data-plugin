using System;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    class DummyImportRecordBuilder : ITestRecord<DummyImportRecord>
    {
        private readonly DummyImportRecord _dummyImportRecord;

        protected DummyImportRecordBuilder()
        {
            _dummyImportRecord = new DummyImportRecord();
        }

        public static DummyImportRecordBuilder StartBuilding()
        {
            return new DummyImportRecordBuilder();
        }

        public DummyImportRecordBuilder WithId(string recordId)
        {
            _dummyImportRecord.Id = recordId;
            return this;
        }

        public DummyImportRecordBuilder WithString(string recordString)
        {
            _dummyImportRecord.RecordString = recordString;
            return this;
        }

        public DummyImportRecordBuilder WithOrdinal(double ordinal)
        {
            _dummyImportRecord.RecordOrdinal = ordinal;
            return this;
        }

        public DummyImportRecordBuilder WithDateTime(DateTime dateTime)
        {
            _dummyImportRecord.RecordDateTime = dateTime;
            return this;
        }

        public DummyImportRecord ARecord()
        {
            return _dummyImportRecord;
        }

        public DummyImportRecord AParametricRecord(int ordinal)
        {
            return WithId(Invariant($"locID{ordinal}"))
                    .WithString(Invariant($"loc{ordinal}"))
                    .WithOrdinal(ordinal)
                    .WithDateTime(new DateTime())
                    .ARecord();
        }
    }
}
