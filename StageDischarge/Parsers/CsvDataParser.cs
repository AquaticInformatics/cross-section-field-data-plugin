using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FileHelpers;
using FileHelpers.Events;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.Interfaces;

namespace Server.Plugins.FieldVisit.StageDischarge.Parsers
{
    public class CsvDataParser<T> : IDataParser<T> where T : class, ISelfValidator
    {
        private Regex HeaderRegex { get; set; }

        public double InvalidRecords { get; private set; }
        public double ValidRecords { get; private set; }
        public double SkippedRecords { get; private set; }

        
        public IEnumerable<T> ParseInputData(Stream inputStream)
        {
            using (var reader = new StreamReader(inputStream))
            {
                var parseEngine = CreateParserEngine();
                CreateHeaderRegex(parseEngine);
                return parseEngine.ReadStream(reader);
            }
        }

        private DelimitedFileEngine<T> CreateParserEngine()
        {
            var parseEngine = new DelimitedFileEngine<T> {ErrorMode = ErrorMode.ThrowException};
            parseEngine.BeforeReadRecord += BeforeReadRecord;
            parseEngine.AfterReadRecord += AfterReadRecord;
            return parseEngine;
        }

        private void CreateHeaderRegex(DelimitedFileEngine<T> engine)
        {
            var pattern = $"^\\s*{string.Join($"\\s*{engine.Options.Delimiter}\\s*", engine.Options.FieldsNames.Where(n => !n.StartsWith("Reading")))}.*$";
            HeaderRegex = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private void AfterReadRecord(EngineBase engine, AfterReadEventArgs<T> e)
        {
            ++InvalidRecords;
            e.Record.Validate();
            --InvalidRecords;
            ++ValidRecords;
        }

        private void BeforeReadRecord(EngineBase engine, BeforeReadEventArgs<T> e)
        {
            if (!string.IsNullOrWhiteSpace(e.RecordLine) 
                && !HeaderRegex.IsMatch(e.RecordLine) 
                && !e.RecordLine.StartsWith(CsvParserConstants.CommentMarker))
                return;

            ++SkippedRecords;
            e.SkipThisRecord = true;
        }
    }
}
