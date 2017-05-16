using System;
using System.IO;
using log4net;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;
using DataModel = Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection
{
    public class CrossSectionPlugin : IFieldDataPlugin
    {
        public ParseFileStatus ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new UnknownLocationHandler(fieldDataResultsAppender);

            return ParseFile(fileStream, fieldDataResultsAppender, fieldVisitHandler);
        }

        public ParseFileStatus ParseFile(Stream fileStream, ILocation selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new KnownLocationHandler(selectedLocation, fieldDataResultsAppender);

            return ParseFile(fileStream, fieldDataResultsAppender, fieldVisitHandler);
        }

        public ParseFileStatus ParseFile(Stream fileStream, IFieldVisit fieldVisit, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new KnownFieldVisitHandler(fieldVisit);

            return ParseFile(fileStream, fieldDataResultsAppender, fieldVisitHandler);
        }

        private static ParseFileStatus ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            IFieldVisitHandler fieldVisitHandler)
        {
            try
            {
                var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
                var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

                var locationIdentifier = GetLocationIdentifier(parsedFileContents);
                var fieldVisitInfo = fieldVisitHandler.GetFieldVisit(locationIdentifier, crossSectionSurvey);

                fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);

                return ParseFileStatus.Succeeded;
            }
            catch (Exception)
            {
                return ParseFileStatus.Failed;
            }
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
