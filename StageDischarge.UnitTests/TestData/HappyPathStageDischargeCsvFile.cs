using System;
using System.IO;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData
{
    public class StageDischargeCsvFileBuilder
    {
        public static MemoryStream CreateCsvFile(IFixture fixture)
        {
            InMemoryCsvFile<StageDischargeRecord> csvFile = new InMemoryCsvFile<StageDischargeRecord>();
            csvFile.AddRecord(CreateFullRecord(fixture));
            return csvFile.GetInMemoryCsvFileStream();
        }

        public static StageDischargeRecord CreateFullRecord(IFixture fixture)
        {
            return StageDischargeRecordBuilder.Build()
                    .WithLocationIdentifier(fixture.Create<string>())
                    .WithMeasurementId(fixture.Create<string>())
                    .WithMeasurementStartDateTime(DateTime.Now)
                    .WithMeasurementEndDateTime(DateTime.Now.AddHours(2))
                    .WithStageAtStart(fixture.Create<double>())
                    .WithStageAtEnd(fixture.Create<double>())
                    .WithStageUnits("m")
                    .WithDischarge(fixture.Create<double>())
                    .WithDischargeUnits("m^3/s")
                    .WithChannelName(fixture.Create<string>())
                    .WithChannelWidth(fixture.Create<double>())
                    .WithWidthUnits("m")
                    .WithChannelArea(fixture.Create<double>())
                    .WithAreaUnits("m^2")
                    .WithChannelVelocity(fixture.Create<double>())
                    .WithVelocityUnits("m/s")
                    .WithParty(fixture.Create<string>())
                    .WithComments(fixture.Create<string>())
                    .ARecord();
        }
    }
}
