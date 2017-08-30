using System;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    class StageDischargeRecordBuilder
    {
        private readonly StageDischargeRecord _stageDischargeRecord;

        private StageDischargeRecordBuilder()
        {
            _stageDischargeRecord = new StageDischargeRecord();
        }

        public static StageDischargeRecordBuilder Build()
        {
            return new StageDischargeRecordBuilder();
        }

        public StageDischargeRecordBuilder WithLocationIdentifier(string locationIdentifier)
        {
            _stageDischargeRecord.LocationIdentifier = locationIdentifier;
            return this;
        }

        public StageDischargeRecordBuilder WithMeasurementId(string measurementId)
        {
            _stageDischargeRecord.MeasurementId = measurementId;
            return this;
        }
        public StageDischargeRecordBuilder WithMeasurementStartDateTime(DateTimeOffset measurementStartDateTime)
        {
            _stageDischargeRecord.MeasurementStartDateTime = measurementStartDateTime;
            return this;
        }
        public StageDischargeRecordBuilder WithMeasurementEndDateTime(DateTimeOffset measurementEndDateTime)
        {
            _stageDischargeRecord.MeasurementEndDateTime = measurementEndDateTime;
            return this;
        }
        public StageDischargeRecordBuilder WithStageAtStart(double stageAtStart)
        {
            _stageDischargeRecord.StageAtStart = stageAtStart;
            return this;
        }
        public StageDischargeRecordBuilder WithStageAtEnd(double stageAtEnd)
        {
            _stageDischargeRecord.StageAtEnd = stageAtEnd;
            return this;
        }
        public StageDischargeRecordBuilder WithStageUnits(string stageUnits)
        {
            _stageDischargeRecord.StageUnits = stageUnits;
            return this;
        }
        public StageDischargeRecordBuilder WithDischarge(double discharge)
        {
            _stageDischargeRecord.Discharge = discharge;
            return this;
        }
        public StageDischargeRecordBuilder WithDischargeUnits(string dischargeUnits)
        {
            _stageDischargeRecord.DischargeUnits = dischargeUnits;
            return this;
        }
        public StageDischargeRecordBuilder WithChannelName(string channelName)
        {
            _stageDischargeRecord.ChannelName = channelName;
            return this;
        }
        public StageDischargeRecordBuilder WithChannelWidth(double channelWidth)
        {
            _stageDischargeRecord.ChannelWidth = channelWidth;
            return this;
        }
        public StageDischargeRecordBuilder WithWidthUnits(string widthUnits)
        {
            _stageDischargeRecord.WidthUnits = widthUnits;
            return this;
        }
        public StageDischargeRecordBuilder WithChannelArea(double channelArea)
        {
            _stageDischargeRecord.ChannelArea = channelArea;
            return this;
        }
        public StageDischargeRecordBuilder WithAreaUnits(string areaUnits)
        {
            _stageDischargeRecord.AreaUnits = areaUnits;
            return this;
        }
        public StageDischargeRecordBuilder WithChannelVelocity(double channelVelocity)
        {
            _stageDischargeRecord.ChannelVelocity = channelVelocity;
            return this;
        }
        public StageDischargeRecordBuilder WithVelocityUnits(string velocityUnits)
        {
            _stageDischargeRecord.VelocityUnits = velocityUnits;
            return this;
        }
        public StageDischargeRecordBuilder WithParty(string party)
        {
            _stageDischargeRecord.Party = party;
            return this;
        }
        public StageDischargeRecordBuilder WithComments(string comments)
        {
            _stageDischargeRecord.Comments = comments;
            return this;
        }

        public StageDischargeRecord ARecord()
        {
            // todo: replace with a copy/clone of the internal record to
            // avoid situations where the builder is reused rather than 
            // reinitialized
            return _stageDischargeRecord;
        }
    }
}
