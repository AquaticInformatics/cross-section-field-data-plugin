using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using FileHelpers;
using FileHelpers.Events;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Model;
using static Server.Plugins.FieldVisit.CrossSection.Helpers.CrossSectionParserConstants;

namespace Server.Plugins.FieldVisit.CrossSection.Parsers
{
    public class CrossSectionSurveyParser : ICrossSectionParser
    {
        private const string AquariusCrossSectionCsvHeader = Header;
        private CrossSectionSurvey CrossSectionSurvey { get; set; }

        public CrossSectionSurvey ParseFile(Stream fileStream)
        {
            CreateCrossSectionSurvey();

            CrossSectionSurvey.Points = ParseCrossSectionFile(fileStream);

            return CrossSectionSurvey;
        }

        private void CreateCrossSectionSurvey()
        {
            CrossSectionSurvey = new CrossSectionSurvey();
        }

        private List<CrossSectionPoint> ParseCrossSectionFile(Stream fileStream)
        {
            using (var reader = CreateStreamReader(fileStream))
            {
                VerifyIsCrossSectionCsvFile(reader);

                return ParsePoints(reader);
            }
        }

        private static StreamReader CreateStreamReader(Stream fileStream)
        {
            const int defaultByteBufferSize = 1024;

            return new StreamReader(fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
                bufferSize: defaultByteBufferSize, leaveOpen: true);
        }

        private void VerifyIsCrossSectionCsvFile(TextReader reader)
        {
            var firstLine = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(firstLine) || !IsHeaderRecord(firstLine))
                throw new FormatNotSupportedException("Uploaded file is not an AQUARIUS Cross-Section");

            ParseFileVersion(firstLine);
        }

        private static bool IsHeaderRecord(string header)
        {
            return header.StartsWith(AquariusCrossSectionCsvHeader, StringComparison.OrdinalIgnoreCase);
        }

        private void ParseFileVersion(string headerLine)
        {
            const string anyDigitFollowedByPeriodAndDigits = @"\d+(\.\d+)+";
            var versionNumeberRegex = new Regex(anyDigitFollowedByPeriodAndDigits, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var versionNumber = versionNumeberRegex.Match(headerLine);

            if (!versionNumber.Success)
                return;

            Version version;

            if (Version.TryParse(versionNumber.Value, out version))
                CrossSectionSurvey.CsvFileVersion = version;
        }

        private List<CrossSectionPoint> ParsePoints(StreamReader reader)
        {
            var engine = CreateCsvParser(reader);

            const int parseAllRecords = -1;
            return engine.ReadStreamAsList(reader, parseAllRecords);
        }

        private DelimitedFileEngine<CrossSectionPoint> CreateCsvParser(StreamReader reader)
        {
            var engine = new DelimitedFileEngine<CrossSectionPoint>(reader.CurrentEncoding);
            engine.Options.IgnoreEmptyLines = true;
            engine.BeforeReadRecord += OnBeforeReadRecord;

            return engine;
        }

        private void OnBeforeReadRecord(EngineBase engine, BeforeReadEventArgs<CrossSectionPoint> eventArgs)
        {
            var line = eventArgs.RecordLine;
            eventArgs.SkipThisRecord = true;

            if (string.IsNullOrWhiteSpace(line) || IsHeaderRecord(line))
            {
                return;
            }

            if (IsMetadataRecord(line))
            {
                ParseMetadata(line);
                return;
            }

            if (IsDataRecord(line))
            {
                eventArgs.SkipThisRecord = false;
            }
        }

        private static bool IsMetadataRecord(string line)
        {
            return line.Contains(MetadataSeparator);
        }

        private void ParseMetadata(string line)
        {
            var metadata = ParseMetadataRecord(line);

            if (CrossSectionSurvey.Metadata.ContainsKey(metadata.Key))
                throw new ParsingFailedException(FormattableString.Invariant($"File has duplicate {metadata.Key} records"));

            CrossSectionSurvey.Metadata.Add(metadata.Key, metadata.Value);
        }

        public KeyValuePair<string, string> ParseMetadataRecord(string line)
        {
            var metaDataSeparatorIndex = line.IndexOf(MetadataSeparator, StringComparison.Ordinal);

            var header = line.Substring(0, metaDataSeparatorIndex).Trim();
            var data = line.Substring(metaDataSeparatorIndex + 1).Trim();

            return new KeyValuePair<string, string>(header, data);
        }

        private static bool IsDataRecord(string line)
        {
            return char.IsNumber(line, 0) && line.Contains(DataRecordSeparator);
        }
    }
}
