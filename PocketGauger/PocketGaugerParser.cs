using System;
using System.IO;
using System.IO.Compression;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;
using static System.FormattableString;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser : IFieldDataPlugin
    {
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            try
            {
                using (var zipArchive = GetZipArchive(fileStream))
                using (var zipContents = GetZipContents(zipArchive))
                {
                    if (!zipContents.ContainsKey(FileNames.GaugingSummary))
                    {
                        throw new PocketGaugerZipFileMissingRequiredContentException(
                            Invariant($"Zip file does not contain file {FileNames.GaugingSummary}"));
                    }

                    var gaugingSummary = CreateGaugingSummaryAssembler().Assemble(zipContents);

                    ProcessGaugingSummary(gaugingSummary, fieldDataResultsAppender);
                }

                return ParseFileResult.ParsedSuccessfully();
            }
            catch (Exception ex)
                when (ex is PocketGaugerZipFileMissingRequiredContentException || ex is PocketGaugerZipFileException)
            {
                return ParseFileResult.CannotParse();
            }
            catch (PocketGaugerDataFormatException e)
            {
                return ParseFileResult.DataInvalid(e);
            }
            catch (Exception e)
            {
                return ParseFileResult.ParsingFailed(e);
            }
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }

        private static ZipArchive GetZipArchive(Stream fileStream)
        {
            try
            {
                return new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen: true);
            }
            catch (InvalidDataException)
            {
                throw new PocketGaugerZipFileException("File is not a zip file");
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
            try
            {
                var dischargeActivityMapper = CreateDischargeActivityMapper();

                foreach (var gaugingSummaryItem in gaugingSummary.GaugingSummaryItems)
                {
                    var locationIdentifier = gaugingSummaryItem.SiteId;
                    var location = fieldDataResultsAppender.GetLocationByIdentifier(locationIdentifier);

                    var dischargeActivity = dischargeActivityMapper.Map(gaugingSummaryItem, location.UtcOffset);

                    var fieldVisit = CreateFieldVisit(dischargeActivity);
                    var fieldVisitInfo = fieldDataResultsAppender.AddFieldVisit(location, fieldVisit);

                    fieldDataResultsAppender.AddDischargeActivity(fieldVisitInfo, dischargeActivity);
                }
            }
            catch (Exception e)
            {
                throw new PocketGaugerDataPersistanceException("Failed to persist pocket gauger data", e);
            }

        }

        private static DischargeActivityMapper CreateDischargeActivityMapper()
        {
            var meterCalibrationMapper = new MeterCalibrationMapper();
            var verticalMapper = new VerticalMapper(meterCalibrationMapper);
            var pointVelocityMapper = new PointVelocityMapper(verticalMapper);
            return new DischargeActivityMapper(pointVelocityMapper);
        }

        private static FieldVisitDetails CreateFieldVisit(DischargeActivity dischargeActivity)
        {
            return new FieldVisitDetails(dischargeActivity.MeasurementPeriod)
            {
                Party = dischargeActivity.Party
            };
        }
    }
}
