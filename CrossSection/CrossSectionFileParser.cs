using System;
using System.IO;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.Plugins.FieldVisit.CrossSection.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;
using DataModel = Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection
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

        private static DataModel.CrossSection.CrossSectionSurvey MapToCrossSectionSurvey(CrossSectionSurvey parsedFileContents)
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

            _logger.Info(Invariant($"Parsed cross-section survey with {crossSectionSurvey.Points.Count} points"));

            return crossSectionSurvey;
        }
    }
}
