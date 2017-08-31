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
                .ToDictionary(r => _fieldDataResultsAppender.GetLocationByIdentifier(r.Key),
                              v => v.OrderBy(x => x.MeasurementStartDateTime).ToList());

            foreach (var locationInfo in sortedRecordsByLocation.Keys.OrderBy(l => l.LocationIdentifier))
            {
                CreateVisitsAndDischargeActivities(locationInfo, sortedRecordsByLocation[locationInfo]);
            }
        }

        private void CreateVisitsAndDischargeActivities(LocationInfo location, IEnumerable<StageDischargeRecord> locationRecords)
        {
            foreach (var stageDischargeRecord in locationRecords)
            {
                var fieldVisit = CreateVisit(location, stageDischargeRecord);
                CreateDischargeActivityForVisit(fieldVisit, stageDischargeRecord);
            }
        }

        private FieldVisitInfo CreateVisit(LocationInfo location, StageDischargeRecord visitRecord)
        {
            var visitStart = visitRecord.MeasurementStartDateTime;
            var visitEnd = visitRecord.MeasurementEndDateTime;

            var fieldVisitDetails = new FieldVisitDetails(new DateTimeInterval(visitStart, visitEnd))
            {
                Comments = visitRecord.Comments,
                Party = visitRecord.Party
            };

            return _fieldDataResultsAppender.AddFieldVisit(location, fieldVisitDetails);
        }

        private void CreateDischargeActivityForVisit(FieldVisitInfo fieldVisit, StageDischargeRecord visitRecord)
        {
            _fieldDataResultsAppender.AddDischargeActivity(fieldVisit, CreateDischargeActivityFromRecord(visitRecord));
        }

        private DischargeActivity CreateDischargeActivityFromRecord(StageDischargeRecord record)
        {
            return _dischargeActivityMapper.FromStageDischargeRecord(record);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }
    }
}
