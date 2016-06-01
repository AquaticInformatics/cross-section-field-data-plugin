using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using static System.FormattableString;
using MeterCalibration = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterCalibration;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser : IFieldDataPlugIn
    {
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            using (var zipArchive = GetZipArchive(fileStream))
            using (var zipContents = GetZipContents(zipArchive))
            {
                if (!zipContents.ContainsKey(FileNames.GaugingSummary))
                {
                    throw new FormatNotSupportedException(
                        Invariant($"Zip file does not contain file {FileNames.GaugingSummary}"));
                }

                var gaugingSummary = GaugingSummaryParser.Parse(zipContents);
                var panels = PanelParser.Parse(zipContents);

                var meterCalibrations = CreateMeterCalibrations(context, zipContents);

                return CreateParsedResults(context, gaugingSummary);
            }
        }

        private static IReadOnlyDictionary<string, MeterCalibration> CreateMeterCalibrations(IParseContext context,
            PocketGaugerFiles zipContents)
        {
            var pocketGaugerMeters = MeterDetailsParser.Parse(zipContents);
            var fieldVisitMeters = new MeterCalibrationMapper(context).Map(pocketGaugerMeters);

            return fieldVisitMeters;
        }

        private static ZipArchive GetZipArchive(Stream fileStream)
        {
            try
            {
                return new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen:true);
            }
            catch (InvalidDataException)
            {
                throw new FormatNotSupportedException("fileStream is not a zip file");
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

        public List<ParsedResult> CreateParsedResults(IParseContext context, GaugingSummary gaugingSummary)
        {
            var parsedResults = new List<ParsedResult>();
            foreach (var gaugingSummaryItem in gaugingSummary.GaugingSummaryItems)
            {
                var locationInfo = GetLocationInfoOrThrow(context, gaugingSummaryItem.SiteId);

                var dischargeActivity = CreateDischargeActivity(locationInfo, gaugingSummaryItem, context);

                parsedResults.Add(CreateParsedResult(locationInfo, dischargeActivity));
            }

            return parsedResults;
        }

        private static ILocationInfo GetLocationInfoOrThrow(IParseContext context, string locationIdentifier)
        {
            var locationInfo = context.FindLocationByIdentifier(locationIdentifier);

            if (locationInfo == null)
            {
                throw new ParsingFailedException(
                    Invariant($"Location with identifier: {locationIdentifier} does not exist"));
            }

            return locationInfo;
        }

        private static DischargeActivity CreateDischargeActivity(ILocationInfo locationInfo,
            GaugingSummaryItem gaugingSummaryItem, IParseContext context)
        {
            return new DischargeActivity
            {
                StartTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.StartDate, locationInfo),
                EndTime = CreateLocationBasedDateTimeOffset(gaugingSummaryItem.EndDate, locationInfo),
                Party = gaugingSummaryItem.ObserversName,
                DischargeUnit = context.DischargeParameter.DefaultUnit,
                GageHeightUnit = context.GageHeightParameter.DefaultUnit,
                DischargeMethod = GetDischargeMonitoringMethod(gaugingSummaryItem.FlowCalculationMethod, context),
                GageHeightMethod = context.GetDefaultMonitoringMethod()
            };
        }

        private static DateTimeOffset CreateLocationBasedDateTimeOffset(DateTime dateTime, ILocationInfo locationInfo)
        {
            return new DateTimeOffset(dateTime, TimeSpan.FromHours(locationInfo.UtcOffsetHours));
        }

        private static IMonitoringMethod GetDischargeMonitoringMethod(FlowCalculationMethod? gaugingMethod, IParseContext context)
        {
            switch (gaugingMethod)
            {
                case FlowCalculationMethod.Mean:
                    return context.DischargeParameter.GetMonitoringMethod(ParametersAndMethodsConstants.MeanSectionMonitoringMethod);
                case FlowCalculationMethod.Mid:
                    return context.DischargeParameter.GetMonitoringMethod(ParametersAndMethodsConstants.MidSectionMonitoringMethod);
                default:
                    return context.GetDefaultMonitoringMethod();
            }
        }
        private static ParsedResult CreateParsedResult(ILocationInfo locationInfo, DischargeActivity dischargeActivity)
        {
            var existingFieldVisits = locationInfo.FindLocationFieldVisitsInTimeRange(dischargeActivity.StartTime, dischargeActivity.EndTime);

            if (existingFieldVisits.Any())
            {
                return CreateNewDischargeActivityParsedResult(dischargeActivity, existingFieldVisits.First());
            }

            return CreateNewFieldVisit(locationInfo, dischargeActivity);
        }

        private static NewFieldVisit CreateNewFieldVisit(ILocationInfo locationInfo, DischargeActivity dischargeActivity)
        {
            return new NewFieldVisit
            {
                Location = locationInfo,
                FieldVisit = new BusinessInterfaces.FieldDataPlugInCore.DataModel.FieldVisit
                {
                    StartDate = dischargeActivity.StartTime,
                    EndDate = dischargeActivity.EndTime,
                    Party = dischargeActivity.Party,
                    DischargeActivities = new[] { dischargeActivity }
                }
            };
        }

        private static NewDischargeActivity CreateNewDischargeActivityParsedResult(DischargeActivity dischargeActivity,
            IFieldVisitInfo fieldVisitInfo)
        {
            return new NewDischargeActivity
            {
                DischargeActivity = dischargeActivity,
                FieldVisit = fieldVisitInfo
            };
        }
    }
}
