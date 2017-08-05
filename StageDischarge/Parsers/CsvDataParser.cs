using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using FileHelpers;
using FileHelpers.Events;
using Server.Plugins.FieldVisit.StageDischarge.Helpers;

namespace Server.Plugins.FieldVisit.StageDischarge.Parsers
{
    class CsvDataParser<T> where T : class, ISelfValidator
    {
        private Regex HeaderRegex { get; set; }

        public double InvalidRecords { get; private set; }
        public double ValidRecords { get; private set; }
        public double SkippedRecords { get; private set; }

        
        public IEnumerable<T> ParseCsvData(Stream inputStream)
        {
            using (var reader = new StreamReader(inputStream))
            {
                var parseEngine = CreateParserEngine();
                CreateHeaderRegex(parseEngine);

                // might be better to handle each record on a line-by-line basis
                // rather than to buffer all of them and then process them.
                var inputRecords = parseEngine.ReadStream(reader);
                foreach (var record in inputRecords)
                {
                    // something something...
                    Console.WriteLine(record);

                }

                return inputRecords;
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
            var pattern = $"^\\s*{string.Join($"\\s*{engine.Options.Delimiter}\\s*", engine.Options.FieldsNames)}\\s*$";
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
            // stolen from Doug - thanks Doug!
            if (!string.IsNullOrWhiteSpace(e.RecordLine) 
                && !HeaderRegex.IsMatch(e.RecordLine) 
                && !e.RecordLine.StartsWith(CsvParserConstants.CommentMarker))
                return;

            ++SkippedRecords;
            e.SkipThisRecord = true;
        }
    }
}
