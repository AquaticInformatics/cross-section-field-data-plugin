using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileHelpers;
using FileHelpers.Events;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
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
                ParseVersionFromHeader(reader);

                return ParsePointData(reader);
            }
        }

        private static StreamReader CreateStreamReader(Stream fileStream)
        {
            const int defaultByteBufferSize = 1024;

            return new StreamReader(fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true,
                bufferSize: defaultByteBufferSize, leaveOpen: true);
        }

        private void ParseVersionFromHeader(TextReader reader)
        {
            var firstLine = reader.ReadLine();

            VerifyIsCrossSectionCsvFile(firstLine);

            ParseFileVersion(firstLine);
        }

        private static void VerifyIsCrossSectionCsvFile(string firstLine)
        {
            if (string.IsNullOrWhiteSpace(firstLine) || !IsHeaderRecord(firstLine))
                throw new FormatNotSupportedException("Uploaded file is not an AQUARIUS Cross-Section");
        }

        private static bool IsHeaderRecord(string header)
        {
            return header.StartsWith(AquariusCrossSectionCsvHeader, StringComparison.OrdinalIgnoreCase);
        }

        private void ParseFileVersion(string headerLine)
        {
            const string anyDigitFollowedByPeriodAndDigits = @"\d+(\.\d+)+";
            var versionNumberRegex = new Regex(anyDigitFollowedByPeriodAndDigits, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var versionNumber = versionNumberRegex.Match(headerLine);

            if (!versionNumber.Success)
                return;

            Version version;

            if (Version.TryParse(versionNumber.Value, out version))
                CrossSectionSurvey.CsvFileVersion = version;
        }

        private List<CrossSectionPoint> ParsePointData(StreamReader reader)
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

            if (IsCrossSectionSurveyData(line))
            {
                ParseCrossSectionSurveyData(line);
                return;
            }

            if (IsCrossSectionPointData(line))
            {
                eventArgs.SkipThisRecord = false;
            }
        }

        private static bool IsCrossSectionSurveyData(string line)
        {
            return line.Contains(CrossSectionDataSeparator);
        }

        private void ParseCrossSectionSurveyData(string line)
        {
            var data = ParseCrossSectionData(line);

            if (CrossSectionSurvey.Fields.ContainsKey(data.Key))
                throw new ParsingFailedException(FormattableString.Invariant($"File has duplicate {data.Key} records"));

            CrossSectionSurvey.Fields.Add(data.Key, data.Value);
        }

        private static KeyValuePair<string, string> ParseCrossSectionData(string line)
        {
            var separatorIndex = line.IndexOf(CrossSectionDataSeparator, StringComparison.Ordinal);

            var field = line.Substring(0, separatorIndex).Trim();
            var data = line.Substring(separatorIndex + 1).Trim();

            return new KeyValuePair<string, string>(field, data);
        }

        private static bool IsCrossSectionPointData(string line)
        {
            var lineTokens = line.Split(CrossSectionPointDataSeparator.ToCharArray());

            return DoubleHelper.Parse(lineTokens.FirstOrDefault()).HasValue;
        }
    }
}
