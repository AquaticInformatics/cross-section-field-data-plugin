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

        public int ValidRecords { get; private set; }
        public int SkippedRecords { get; private set; }
        public string[] Errors { get; private set; }
        
        public IEnumerable<T> ParseInputData(Stream inputStream)
        {
            using (var reader = new StreamReader(inputStream))
            {
                var parseEngine = CreateParserEngine();
                CreateHeaderRegex(parseEngine);

                var records = parseEngine.ReadStream(reader);

                Errors = parseEngine.ErrorManager.Errors
                    .Select(e => $"Line {e.LineNumber}: {e.ExceptionInfo.Message}")
                    .ToArray();

                return records;
            }
        }

        private DelimitedFileEngine<T> CreateParserEngine()
        {
            var parseEngine = new DelimitedFileEngine<T> {ErrorMode = ErrorMode.SaveAndContinue};
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
            e.Record.Validate();
            ++ValidRecords;
        }

        private void BeforeReadRecord(EngineBase engine, BeforeReadEventArgs<T> e)
        {
            var line = e.RecordLine.Trim();

            if (!string.IsNullOrWhiteSpace(line) 
                && !HeaderRegex.IsMatch(line) 
                && !line.StartsWith(CsvParserConstants.CommentMarker))
                return;

            ++SkippedRecords;
            e.SkipThisRecord = true;
        }
    }
}
