using System.IO;
using System.IO.Compression;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;
using DataModel = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using static System.FormattableString;


namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser : IFieldDataPlugIn
    {
        public void ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            using (var zipArchive = GetZipArchive(fileStream))
            using (var zipContents = GetZipContents(zipArchive))
            {
                if (!zipContents.ContainsKey(FileNames.GaugingSummary))
                {
                    throw new FormatNotSupportedException(
                        Invariant($"Zip file does not contain file {FileNames.GaugingSummary}"));
                }

                var gaugingSummary = CreateGaugingSummaryAssembler().Assemble(zipContents);

                ProcessGaugingSummary(gaugingSummary, fieldDataResultsAppender);
            }
        }

        public void ParseFile(Stream fileStream, string locationIdentifier, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            ParseFile(fileStream, fieldDataResultsAppender, logger);
        }

        public void ParseFile(Stream fileStream, IFieldVisitInfo fieldVisitInfo, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            ParseFile(fileStream, fieldDataResultsAppender, logger);
        }

        private static ZipArchive GetZipArchive(Stream fileStream)
        {
            try
            {
                return new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen: true);
            }
            catch (InvalidDataException)
            {
                throw new FormatNotSupportedException("File is not a zip file");
            }
        }

        private static PocketGaugerFiles GetZipContents(ZipArchive zipArchive)
        {
            var streams = new PocketGaugerFiles();
            foreach (var zipArchiveEntry in zipArchive.Entries)
            {
                streams.Add(zipArchiveEntry.Name.ToLower(), zipArchiveEntry.Open());
            }

            return streams;
        }

        private static GaugingSummaryAssembler CreateGaugingSummaryAssembler()
        {
            return new GaugingSummaryAssembler(new GaugingSummaryParser(), new MeterDetailsParser(), new PanelParser());
        }

        public void ProcessGaugingSummary(GaugingSummary gaugingSummary, IFieldDataResultsAppender fieldDataResultsAppender)
        {
            var dischargeActivityMapper = CreateDischargeActivityMapper();

            foreach (var gaugingSummaryItem in gaugingSummary.GaugingSummaryItems)
            {
                var locationIdentifier = gaugingSummaryItem.SiteId;
                var timeZoneOffsetAtLocation = fieldDataResultsAppender.GetTimeZoneOffsetAtLocation(locationIdentifier);

                var dischargeActivity = dischargeActivityMapper.Map(gaugingSummaryItem, timeZoneOffsetAtLocation);

                var fieldVisit = CreateFieldVisit(dischargeActivity);
                var fieldVisitInfo = fieldDataResultsAppender.AddFieldVisit(locationIdentifier, fieldVisit);

                fieldDataResultsAppender.AddDischargeActivity(fieldVisitInfo, dischargeActivity);
            }
        }

        private static DischargeActivityMapper CreateDischargeActivityMapper()
        {
            var meterCalibrationMapper = new MeterCalibrationMapper();
            var verticalMapper = new VerticalMapper(meterCalibrationMapper);
            var pointVelocityMapper = new PointVelocityMapper(verticalMapper);
            return new DischargeActivityMapper(pointVelocityMapper);
        }

        private static DataModel.FieldVisit CreateFieldVisit(DischargeActivity dischargeActivity)
        {
            return new DataModel.FieldVisit
            {
                StartDate = dischargeActivity.StartTime,
                EndDate = dischargeActivity.EndTime,
                Party = dischargeActivity.Party
            };
        }
    }
}
