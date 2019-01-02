using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;
using CrossSectionPlugin.Interfaces;
using CrossSectionPlugin.Model;
using FileHelpers;
using FileHelpers.Events;
using static CrossSectionPlugin.Helpers.CrossSectionParserConstants;
using static System.FormattableString;

namespace CrossSectionPlugin.Parsers
{
    public class CrossSectionSurveyParser : ICrossSectionParser
    {
        private const string AquariusCrossSectionCsvHeader = Header;
        public const string CannotParseFileVersion = "Cannot parse the version from the file header";
        private CrossSectionSurvey CrossSectionSurvey { get; set; }
        private bool _isPointOrderSpecified;

        public CrossSectionSurvey ParseFile(Stream fileStream)
        {
            CreateCrossSectionSurvey();

            ParseCrossSectionFile(fileStream);

            return CrossSectionSurvey;
        }

        private void CreateCrossSectionSurvey()
        {
            CrossSectionSurvey = new CrossSectionSurvey();
        }

        private void ParseCrossSectionFile(Stream fileStream)
        {
            using (var reader = CreateStreamReader(fileStream))
            {
                ParseVersionFromHeader(reader);

                _isPointOrderSpecified = IsPointOrderSpecifiedOrInferred(reader);
            }

            fileStream.Position = 0;

            using (var reader = CreateStreamReader(fileStream))
            {
                ParsePoints(reader);
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
                throw new CrossSectionCsvFormatException("Uploaded file is not an AQUARIUS Cross-Section");
        }

        private static bool IsHeaderRecord(string header)
        {
            return header.StartsWith(AquariusCrossSectionCsvHeader, StringComparison.InvariantCultureIgnoreCase);
        }

        private void ParseFileVersion(string headerLine)
        {
            const string anyDigitFollowedByPeriodAndDigits = @"\d+(\.\d+)+";
            var versionNumberRegex = new Regex(anyDigitFollowedByPeriodAndDigits, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            var versionNumber = versionNumberRegex.Match(headerLine);

            if (!versionNumber.Success)
                throw new CrossSectionSurveyDataFormatException(CannotParseFileVersion);

            if (Version.TryParse(versionNumber.Value, out var version))
                CrossSectionSurvey.CsvFileVersion = version;
        }

        private static bool IsPointOrderSpecifiedOrInferred(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var lineTokens = line.Split(CrossSectionPointDataSeparator.ToCharArray());

                if (lineTokens.First().Equals("PointOrder", StringComparison.InvariantCultureIgnoreCase)) return true;
            }

            return false;
        }

        private void ParsePoints(StreamReader reader)
        {
            switch (CrossSectionSurvey.CsvFileVersion.Major)
            {
                case 1:
                {
                    if (_isPointOrderSpecified) throw new CrossSectionSurveyDataFormatException(
                        Invariant($"{CrossSectionSurvey.CsvFileVersion} does not support \"PointOrder\" as a column"));
                    CrossSectionSurvey.Points = ParsePointData<CrossSectionPointV1>(reader);
                    break;
                }
                case 2:
                {
                    if (_isPointOrderSpecified)
                    {
                        CrossSectionSurvey.Points = ParsePointData<CrossSectionPointV2>(reader);
                    }
                    else
                    {
                        var points = ParsePointData<CrossSectionPointV1>(reader);
                        CrossSectionSurvey.Points = ConvertV1PointsToV2(points);
                    }
                    break;
                }
                default:
                {
                    throw new CrossSectionSurveyDataFormatException(
                        Invariant($"{CrossSectionSurvey.CsvFileVersion} is not a supported AQUARIUS Cross-Section file version"));
                }
            }
        }

        private List<ICrossSectionPoint> ParsePointData<TCrossSectionPoint>(StreamReader reader)
            where TCrossSectionPoint : class, ICrossSectionPoint
        {
            var engine = CreateCsvParser<TCrossSectionPoint>(reader);

            const int parseAllRecords = -1;
            var points = engine.ReadStream(reader, parseAllRecords).ToList();

            return new List<ICrossSectionPoint>(points);
        }

        private DelimitedFileEngine<TCrossSectionPoint> CreateCsvParser<TCrossSectionPoint>(StreamReader reader)
            where TCrossSectionPoint : class, ICrossSectionPoint
        {
            var engine = new DelimitedFileEngine<TCrossSectionPoint>(reader.CurrentEncoding);
            engine.Options.IgnoreEmptyLines = true;
            engine.BeforeReadRecord += OnBeforeReadRecord;

            return engine;
        }

        private void OnBeforeReadRecord<TCrossSectionPoint>(EngineBase engine, BeforeReadEventArgs<TCrossSectionPoint> eventArgs)
            where TCrossSectionPoint : class, ICrossSectionPoint
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
                throw new CrossSectionSurveyDataFormatException(Invariant($"File has duplicate {data.Key} records"));

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

        private List<ICrossSectionPoint> ConvertV1PointsToV2(List<ICrossSectionPoint> points)
        {
            var pointOrder = 0;
            var convertedPoints = points.OfType<CrossSectionPointV1>().Select(v1Point => new CrossSectionPointV2
            {
                PointOrder = ++pointOrder,
                Distance = v1Point.Distance,
                Elevation = v1Point.Elevation,
                Comment = v1Point.Comment
            }).Cast<ICrossSectionPoint>().ToList();
            return convertedPoints;
        }
    }
}
