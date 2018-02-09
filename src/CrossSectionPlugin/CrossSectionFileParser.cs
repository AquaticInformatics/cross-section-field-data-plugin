using System;
using System.IO;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;
using CrossSectionPlugin.Interfaces;
using CrossSectionPlugin.Mappers;
using CrossSectionPlugin.Model;
using CrossSectionPlugin.Parsers;
using FieldDataPluginFramework;
using FieldDataPluginFramework.Results;

namespace CrossSectionPlugin
{
    internal class CrossSectionFileParser
    {
        private readonly IFieldDataResultsAppender _fieldDataResultsAppender;
        private readonly IFieldVisitHandler _fieldVisitHandler;
        private readonly ILog _logger;

        public CrossSectionFileParser(IFieldDataResultsAppender fieldDataResultsAppender,
            IFieldVisitHandler fieldVisitHandler, ILog logger)
        {
            _fieldDataResultsAppender = fieldDataResultsAppender;
            _fieldVisitHandler = fieldVisitHandler;
            _logger = logger;
        }

        public ParseFileResult ParseFile(Stream fileStream)
        {
            CrossSectionSurvey parsedFileContents;
            try
            {
                parsedFileContents = ProcessFileStream(fileStream);
            }
            catch (CrossSectionCsvFormatException)
            {
                return ParseFileResult.CannotParse();
            }
            catch (Exception e)
            {
                return ParseFileResult.CannotParse(e);
            }

            try
            {
                return ProcessParsedFileContents(parsedFileContents);
            }
            catch (Exception e)
            {
                return ParseFileResult.SuccessfullyParsedButDataInvalid(e);
            }
        }

        private ParseFileResult ProcessParsedFileContents(CrossSectionSurvey parsedFileContents)
        {
            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var locationIdentifier = GetLocationIdentifier(parsedFileContents);
            var fieldVisitInfo = _fieldVisitHandler.GetFieldVisit(locationIdentifier, crossSectionSurvey);

            _fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);

            return ParseFileResult.SuccessfullyParsedAndDataValid();
        }

        private static FieldDataPluginFramework.DataModel.CrossSection.CrossSectionSurvey MapToCrossSectionSurvey(CrossSectionSurvey parsedFileContents)
        {
            var crossSectionMapper = new CrossSectionMapper(new CrossSectionPointMapper());
            return crossSectionMapper.MapCrossSection(parsedFileContents);
        }

        private static string GetLocationIdentifier(CrossSectionSurvey crossSectionSurvey)
        {
            return crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Location);
        }

        private CrossSectionSurvey ProcessFileStream(Stream fileStream)
        {
            var crossSectionSurvey = new CrossSectionSurveyParser().ParseFile(fileStream);

            _logger.Info(FormattableString.Invariant($"Parsed cross-section survey with {crossSectionSurvey.Points.Count} points"));

            return crossSectionSurvey;
        }
    }
}
