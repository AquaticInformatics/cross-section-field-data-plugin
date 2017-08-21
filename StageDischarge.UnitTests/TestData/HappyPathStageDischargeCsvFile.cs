using System;
using System.IO;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData
{
    public class HappyPathStageDischargeCsvFileBuilder
    {
        public static MemoryStream CreateCsvFile()
        {
            InMemoryCsvFile<StageDischargeRecord> csvFile =
                new InMemoryCsvFile<StageDischargeRecord>();
            csvFile.AddRecord(CreateFullRecord());
            return csvFile.GetInMemoryCsvFileStream();
        }

        public static StageDischargeRecord CreateFullRecord()
        {
            return StageDischargeRecordBuilder.Build()
                    .WithLocationIdentifier("1234")
                    .WithMeasurementId("5678")
                    .WithMeasurementStartDateTime(new DateTime())
                    .WithMeasurementEndDateTime(new DateTime())
                    .WithStageAtStart(11)
                    .WithStageAtEnd(12)
                    .WithStageUnits("m")
                    .WithDischarge(132)
                    .WithDischargeUnits("m^3/s")
                    .WithChannelName("channel1")
                    .WithChannelWidth(10)
                    .WithWidthUnits("m")
                    .WithChannelArea(100)
                    .WithAreaUnits("m^2")
                    .WithChannelVelocity(5)
                    .WithVelocityUnits("m/s")
                    .WithParty("of one")
                    .WithComments("water is wet")
                    .ARecord();
        }
    }
}
