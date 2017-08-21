using System;
using System.Collections.Generic;
using System.IO;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge
{
    public class StageDischargeParser : IFieldDataPlugin
    {
        private CsvDataParser<StageDischargeRecord> parser;

        public StageDischargeParser()
        {
            parser = new CsvDataParser<StageDischargeRecord>();
        }
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            try
            {
                // work the magic here
                var parsedRecords = ParseStreamForRecords(fileStream);

                // do something with the parsed records.

                return ParseFileResult.SuccessfullyParsedAndDataValid();
            }
            catch (Exception e)
            {
                return ParseFileResult.CannotParse(e);
            }
        }

        protected IEnumerable<StageDischargeRecord> ParseStreamForRecords(Stream fileStream)
        {
            return parser.ParseCsvData(fileStream);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            throw new NotImplementedException();
        }
    }
}
