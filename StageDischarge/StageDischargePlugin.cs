using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
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
                if (_parser.InvalidRecords > 0)
                {
                    return ParseFileResult.SuccessfullyParsedButDataInvalid(InputFileContainsInvalidRecords);
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
                .ToDictionary(r => _fieldDataResultsAppender.GetLocationByIdentifier(r.Key),    // convert locationId to LocationInfo
                              v => v.OrderBy(x => x.MeasurementStartDateTime).ToList());        // list of records for each LocationInfo

            foreach (var locationInfo in sortedRecordsByLocation.Keys.OrderBy(l => l.LocationIdentifier))
            {
                CreateVisitsAndDischargeActivities(locationInfo, sortedRecordsByLocation[locationInfo]);
            }
        }

        private void CreateVisitsAndDischargeActivities(LocationInfo location, IEnumerable<StageDischargeRecord> locationRecords)
        {
            var recordsByDate = GetRecordsByDate(locationRecords, location.UtcOffset);

            // create a visit per date. 
            foreach (var startofDay in recordsByDate.Keys.OrderBy(d => d.Ticks))
            {
                var fieldVisit = CreateVisit(location, recordsByDate[startofDay]);
                CreateDischargeActivitiesForVisit(fieldVisit, recordsByDate[startofDay]);
            }
        }

        private static Dictionary<DateTime, List<StageDischargeRecord>> 
            GetRecordsByDate(IEnumerable<StageDischargeRecord> locationRecords, TimeSpan utcOffset)
        {
            var recordsByDate = new Dictionary<DateTime, List<StageDischargeRecord>>();
            foreach (var record in locationRecords)
            {
                var startOfDay = record.MeasurementStartDateTime.Date.Subtract(utcOffset);

                List<StageDischargeRecord> dateRecords;
                if (!recordsByDate.TryGetValue(startOfDay, out dateRecords))
                {
                    dateRecords = new List<StageDischargeRecord>();
                    recordsByDate.Add(startOfDay, dateRecords);
                }

                dateRecords.Add(record);
            }

            return recordsByDate;
        }


        private FieldVisitInfo CreateVisit(LocationInfo location, List<StageDischargeRecord> visitRecordsForLocation)
        {
            var visitStart = visitRecordsForLocation.Min(r => r.MeasurementStartDateTime);
            var visitEnd = visitRecordsForLocation.Max(r => r.MeasurementEndDateTime);

            var fieldVisitDetails = new FieldVisitDetails(new DateTimeInterval(visitStart, visitEnd))
            {
                Comments = GetUniqueStrings(visitRecordsForLocation, r => r.Comments),
                Party = GetUniqueStrings(visitRecordsForLocation, r => r.Party)
            };

            return _fieldDataResultsAppender.AddFieldVisit(location, fieldVisitDetails);
        }

        private void CreateDischargeActivitiesForVisit(FieldVisitInfo fieldVisit, List<StageDischargeRecord> visitRecords)
        {
            var dischargeActivities = visitRecords.Select(CreateDischargeActivityFromRecord).ToList();
            foreach (var dischargeActivity in dischargeActivities)
            {
                _fieldDataResultsAppender.AddDischargeActivity(fieldVisit, dischargeActivity);
            }
        }

        private DischargeActivity CreateDischargeActivityFromRecord(StageDischargeRecord record)
        {
            return _dischargeActivityMapper.FromStageDischargeRecord(record);
        }

        // TODO: examine
        private static string GetUniqueStrings(List<StageDischargeRecord> records, Func<StageDischargeRecord, string> selector)
        {
            return string.Join(", ", records.Select(selector)
                         .Where(s => !string.IsNullOrEmpty(s))
                         .Distinct());
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }
    }
}
