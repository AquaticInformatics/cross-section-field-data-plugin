using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;
using static Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers.StageDischargeRecordBuilder;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData
{
    class HappyPathStageDischargeCsvFileBuilder
    {
        public static MemoryStream CreateCsvFile
        {
            get
            {
                InMemoryCsvFile<StageDischargeRecord> csvFile =
                    new InMemoryCsvFile<StageDischargeRecord>();
                csvFile.AddRecord(CreateRecord());
                return csvFile.GetInMemoryCsvFile();
            }
        }

        private static StageDischargeRecord CreateRecord()
        {
            return CreateBuilder()
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
                    .BuildRecord();
        }
    }
}
