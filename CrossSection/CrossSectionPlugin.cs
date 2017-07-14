using System;
using System.IO;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;
using DataModel = Server.BusinessInterfaces.FieldDataPluginCore.DataModel;

namespace Server.Plugins.FieldVisit.CrossSection
{
    public class CrossSectionPlugin : IFieldDataPlugin
    {
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new UnknownLocationHandler(fieldDataResultsAppender);

            return ParseFile(fileStream, fieldDataResultsAppender, fieldVisitHandler);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new KnownLocationHandler(selectedLocation, fieldDataResultsAppender);

            return ParseFile(fileStream, fieldDataResultsAppender, fieldVisitHandler);
        }

        private static ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            IFieldVisitHandler fieldVisitHandler)
        {
            try
            {
                var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
                return ProcessParsedFileContents(parsedFileContents, fieldDataResultsAppender, fieldVisitHandler);
            }
            catch (CrossSectionCsvFormatException)
            {
                return ParseFileResult.CannotParse();
            }
            catch (Exception e)
            {
                return ParseFileResult.ParsingFailed(e);
            }
        }

        private static ParseFileResult ProcessParsedFileContents(CrossSectionSurvey parsedFileContents,
            IFieldDataResultsAppender fieldDataResultsAppender, IFieldVisitHandler fieldVisitHandler)
        {
            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var locationIdentifier = GetLocationIdentifier(parsedFileContents);
            var fieldVisitInfo = fieldVisitHandler.GetFieldVisit(locationIdentifier, crossSectionSurvey);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);

            return ParseFileResult.ParsedSuccessfully();
        }

        private static DataModel.CrossSection.CrossSectionSurvey MapToCrossSectionSurvey(CrossSectionSurvey parsedFileContents)
        {
            var crossSectionMapper = CreateCrossSectionMapper();
            return crossSectionMapper.MapCrossSection(parsedFileContents);
        }

        private static string GetLocationIdentifier(CrossSectionSurvey crossSectionSurvey)
        {
            return crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Location);
        }

        private static ICrossSectionParser CreateCrossSectionParser()
        {
            return new CrossSectionSurveyParser();
        }

        private static ICrossSectionMapper CreateCrossSectionMapper()
        {
            return new CrossSectionMapper(new CrossSectionPointMapper());
        }

        private static CrossSectionSurvey ProcessFileStream(ICrossSectionParser parser, Stream fileStream)
        {
            return parser.ParseFile(fileStream);
        }
    }
}
