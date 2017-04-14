using System.IO;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;
using DataModel = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection
{
    public class CrossSectionPlugin : IFieldDataPlugIn
    {
        public void ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);

            var locationIdentifier = GetLocationIdentifier(parsedFileContents);
            var location = fieldDataResultsAppender.GetLocationByIdentifier(locationIdentifier);
            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var fieldVisitInfo = CreateFieldVisitForCrossSection(location, crossSectionSurvey, fieldDataResultsAppender);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);
        }

        public void ParseFile(Stream fileStream, ILocation selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
            CheckForExpectedLocation(selectedLocation, parsedFileContents);

            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var fieldVisitInfo = CreateFieldVisitForCrossSection(selectedLocation, crossSectionSurvey, fieldDataResultsAppender);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);
        }

        public void ParseFile(Stream fileStream, IFieldVisit fieldVisit, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
            CheckForExpectedLocation(fieldVisit.Location, parsedFileContents);

            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisit, crossSectionSurvey);
        }

        private static IFieldVisit CreateFieldVisitForCrossSection(ILocation location,
            DataModel.CrossSection.CrossSectionSurvey crossSectionSurvey, IFieldDataResultsAppender fieldDataResultsAppender)
        {
            var fieldVisit = new DataModel.FieldVisitDetails(crossSectionSurvey.SurveyPeriod, crossSectionSurvey.Party);

            return fieldDataResultsAppender.AddFieldVisit(location, fieldVisit);
        }

        private static DataModel.CrossSection.CrossSectionSurvey MapToCrossSectionSurvey(CrossSectionSurvey parsedFileContents)
        {
            var crossSectionMapper = CreateCrossSectionMapper();
            return crossSectionMapper.MapCrossSection(parsedFileContents);
        }

        private static void CheckForExpectedLocation(ILocation selectedLocation, CrossSectionSurvey parsedFileContents)
        {
            var locationIdentifierFromFile = GetLocationIdentifier(parsedFileContents);
            var seletedLocationIdentifier = selectedLocation.LocationIdentifier;

            if (locationIdentifierFromFile.EqualsOrdinalIgnoreCase(seletedLocationIdentifier))
                return;

            throw new ParsingFailedException(
                Invariant($"Location identifier '{seletedLocationIdentifier}' does not match the identifier in the file: '{locationIdentifierFromFile}'"));
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
