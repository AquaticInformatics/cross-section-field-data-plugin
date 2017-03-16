using System.Collections.Generic;
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
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            var crossSectionSurvey = ProcessFileStream(CreateCrossSectionParser(), fileStream);

            var parsedResult = CreateParsedResult(CreateParsedResultMapper(context), crossSectionSurvey);

            return new[] { parsedResult };
        }

        public void ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);

            var locationIdentifier = GetLocationIdentifier(parsedFileContents);
            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var fieldVisitInfo = CreateFieldVisitForCrossSection(locationIdentifier, crossSectionSurvey, fieldDataResultsAppender);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);
        }

        public void ParseFile(Stream fileStream, string locationIdentifier, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
            CheckForExpectedLocation(locationIdentifier, parsedFileContents);

            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            var fieldVisitInfo = CreateFieldVisitForCrossSection(locationIdentifier, crossSectionSurvey, fieldDataResultsAppender);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);
        }

        public void ParseFile(Stream fileStream, IFieldVisitInfo fieldVisitInfo, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var parsedFileContents = ProcessFileStream(CreateCrossSectionParser(), fileStream);
            CheckForExpectedLocation(fieldVisitInfo.LocationIdentifier, parsedFileContents);

            var crossSectionSurvey = MapToCrossSectionSurvey(parsedFileContents);

            fieldDataResultsAppender.AddCrossSectionSurvey(fieldVisitInfo, crossSectionSurvey);
        }

        private static IFieldVisitInfo CreateFieldVisitForCrossSection(string locationIdentifier,
            DataModel.CrossSection.CrossSectionSurvey crossSectionSurvey, IFieldDataResultsAppender fieldDataResultsAppender)
        {
            var fieldVisit = new DataModel.FieldVisit
            {
                Party = crossSectionSurvey.Party,
                StartDate = crossSectionSurvey.StartTime,
                EndDate = crossSectionSurvey.EndTime
            };

            return fieldDataResultsAppender.AddFieldVisit(locationIdentifier, fieldVisit);
        }

        private static DataModel.CrossSection.CrossSectionSurvey MapToCrossSectionSurvey(CrossSectionSurvey parsedFileContents)
        {
            var crossSectionMapper = CreateCrossSectionMapper();
            return crossSectionMapper.MapCrossSection(parsedFileContents);
        }

        private static void CheckForExpectedLocation(string locationIdentifier, CrossSectionSurvey parsedFileContents)
        {
            var locationIdentifierFromFile = GetLocationIdentifier(parsedFileContents);

            if (locationIdentifierFromFile.EqualsOrdinalIgnoreCase(locationIdentifier))
                return;

            throw new ParsingFailedException(
                Invariant($"Location identifier '{locationIdentifier}' does not match the identifier in the file: '{locationIdentifierFromFile}'"))
            {
//                TargetLocation = _parseContext.TargetLocation,
//                AmbiguousLocations = new[] { fileLocation }
            };
        }

        private static string GetLocationIdentifier(CrossSectionSurvey crossSectionSurvey)
        {
            return crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Location);
        }

        private static ICrossSectionParser CreateCrossSectionParser()
        {
            return new CrossSectionSurveyParser();
        }

        private static ParsedResult CreateParsedResult(IParsedResultMapper mapper, CrossSectionSurvey crossSectionSurvey)
        {
            return mapper.CreateParsedResult(crossSectionSurvey);
        }

        private static IParsedResultMapper CreateParsedResultMapper(IParseContext context)
        {
            return new ParsedResultMapper(context, CreateCrossSectionMapper(context));
        }

        private static ICrossSectionMapper CreateCrossSectionMapper(IParseContext context)
        {
            return new CrossSectionMapper(context, new CrossSectionPointMapper());
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
