using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FieldDataPluginFramework;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using FieldDataPluginFramework.DataModel.Readings;
using FieldDataPluginFramework.Results;
using Server.Plugins.FieldVisit.StageDischarge.Interfaces;
using Server.Plugins.FieldVisit.StageDischarge.Mappers;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge
{
    public class StageDischargePlugin : IFieldDataPlugin
    {
        public const string NoRecordsInInputFile = "No records found in input file.";
        public const string InputFileContainsInvalidRecords = "Input file contains invalid records.";
        private readonly IDataParser<StageDischargeRecord> _parser;
        private IFieldDataResultsAppender _fieldDataResultsAppender;
        private readonly DischargeActivityMapper _dischargeActivityMapper;

        public StageDischargePlugin() : this(new CsvDataParser<StageDischargeRecord>())
        {}

        public StageDischargePlugin(IDataParser<StageDischargeRecord> parser)
        {
            _parser = parser;
            _dischargeActivityMapper = new DischargeActivityMapper();
        }

        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            _fieldDataResultsAppender = fieldDataResultsAppender;
            try
            {
                var parsedRecords = _parser.ParseInputData(fileStream);
                if (parsedRecords == null)
                {
                    return ParseFileResult.CannotParse(NoRecordsInInputFile);
                }
                if (_parser.Errors.Any())
                {
                    if (_parser.ValidRecords > 0)
                    {
                        return ParseFileResult.SuccessfullyParsedButDataInvalid(
                            $"{InputFileContainsInvalidRecords}: {_parser.Errors.Length} errors:\n{string.Join("\n", _parser.Errors.Take(3))}");
                    }

                    return ParseFileResult.CannotParse();
                }

                logger.Info($"Parsed {_parser.ValidRecords} from input file.");
                SaveRecords(parsedRecords);
                return ParseFileResult.SuccessfullyParsedAndDataValid();
            }
            catch (Exception e)
            {
                logger.Error($"Failed to parse file; {e.Message}");
                return ParseFileResult.CannotParse(e);
            }
        }

        private void SaveRecords(IEnumerable<StageDischargeRecord> parsedRecords)
        {
            var sortedRecordsByLocation = parsedRecords
                .GroupBy(r => r.LocationIdentifier)
                .ToDictionary(r => _fieldDataResultsAppender.GetLocationByIdentifier(r.Key),
                              v => v.OrderBy(x => x.MeasurementStartDateTime).ToList());

            foreach (var locationInfo in sortedRecordsByLocation.Keys.OrderBy(l => l.LocationIdentifier))
            {
                CreateVisitsAndActivities(locationInfo, sortedRecordsByLocation[locationInfo]);
            }
        }

        private void CreateVisitsAndActivities(LocationInfo location, IEnumerable<StageDischargeRecord> locationRecords)
        {
            var createdVisits = new List<FieldVisitInfo>();

            foreach (var stageDischargeRecord in locationRecords)
            {
                var fieldVisit = MergeOrCreateVisit(createdVisits, location, stageDischargeRecord);

                CreateDischargeActivityForVisit(fieldVisit, stageDischargeRecord);
                CreateReadingsForVisit(fieldVisit, stageDischargeRecord);
            }
        }

        private FieldVisitInfo MergeOrCreateVisit(List<FieldVisitInfo> createdVisits, LocationInfo location, StageDischargeRecord visitRecord)
        {
            var existingVisit = createdVisits
                .SingleOrDefault(fv => fv.StartDate.Date == visitRecord.MeasurementStartDateTime.Date);

            if (existingVisit != null)
            {
                MergeWithExistingVisit(existingVisit, visitRecord);

                return existingVisit;
            }

            var visitStart = visitRecord.MeasurementStartDateTime;
            var visitEnd = visitRecord.MeasurementEndDateTime;

            var fieldVisitDetails = new FieldVisitDetails(new DateTimeInterval(visitStart, visitEnd))
            {
                Comments = visitRecord.Comments,
                Party = visitRecord.Party
            };

            var fieldVisitInfo = _fieldDataResultsAppender.AddFieldVisit(location, fieldVisitDetails);

            createdVisits.Add(fieldVisitInfo);

            return fieldVisitInfo;
        }

        private static void MergeWithExistingVisit(FieldVisitInfo existingVisit, StageDischargeRecord visitRecord)
        {
            existingVisit.FieldVisitDetails.FieldVisitPeriod = ExpandInterval(
                existingVisit.FieldVisitDetails.FieldVisitPeriod,
                visitRecord.MeasurementStartDateTime,
                visitRecord.MeasurementEndDateTime);

            existingVisit.FieldVisitDetails.Comments =
                MergeUniqueComments(existingVisit.FieldVisitDetails.Comments, visitRecord.Comments);

            existingVisit.FieldVisitDetails.Party =
                MergeUniqueParties(existingVisit.FieldVisitDetails.Party, visitRecord.Party);
        }

        private static string MergeUniqueComments(params string[] values)
        {
            return MergeUniqueStrings("\n", values);
        }

        private static string MergeUniqueParties(params string[] values)
        {
            return MergeUniqueStrings(", ", values);
        }

        private static string MergeUniqueStrings(string separator, string[] values)
        {
            return string.Join(separator, values.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct());
        }

        private static DateTimeInterval ExpandInterval(DateTimeInterval interval, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            var minStart = interval.Start < startTime
                ? interval.Start
                : startTime;

            var maxEnd = interval.End > endTime
                ? interval.End
                : endTime;

            return new DateTimeInterval(minStart, maxEnd);
        }

        private void CreateDischargeActivityForVisit(FieldVisitInfo fieldVisit, StageDischargeRecord record)
        {
            if (!record.Discharge.HasValue) return;

            _fieldDataResultsAppender.AddDischargeActivity(fieldVisit, CreateDischargeActivityFromRecord(record));
        }

        private DischargeActivity CreateDischargeActivityFromRecord(StageDischargeRecord record)
        {
            return _dischargeActivityMapper.FromStageDischargeRecord(record);
        }

        private void CreateReadingsForVisit(FieldVisitInfo fieldVisit, StageDischargeRecord record)
        {
            var readingTime = GetHumanReadableMidpoint(
                new DateTimeInterval(record.MeasurementStartDateTime, record.MeasurementEndDateTime));

            foreach (var reading in record.Readings)
            {
                var parameterReading = new Reading(reading.Parameter, new Measurement(reading.Value, reading.Units))
                {
                    Comments = record.Comments,
                    DateTimeOffset = readingTime
                };

                _fieldDataResultsAppender.AddReading(fieldVisit, parameterReading);
            }
        }

        private DateTimeOffset GetHumanReadableMidpoint(DateTimeInterval interval)
        {
            var duration = interval.End - interval.Start;
            var midpoint = interval.Start + TimeSpan.FromTicks(duration.Ticks / 2);

            var truncatedTime = new DateTimeOffset(
                midpoint.Year,
                midpoint.Month,
                midpoint.Day,
                midpoint.Hour,
                midpoint.Minute,
                0,
                midpoint.Offset);

            return truncatedTime < interval.Start ? interval.Start : truncatedTime;
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }
    }
}
