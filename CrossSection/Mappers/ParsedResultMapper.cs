using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using CrossSectionSurvey = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection.CrossSectionSurvey;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection.Mappers
{
    public class ParsedResultMapper : IParsedResultMapper
    {
        private readonly IParseContext _parseContext;
        private readonly ICrossSectionMapper _crossSectionMapper;

        public ParsedResultMapper(IParseContext parseContext, ICrossSectionMapper crossSectionMapper)
        {
            _parseContext = parseContext;
            _crossSectionMapper = crossSectionMapper;
        }

        public ParsedResult CreateParsedResult(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var locationInfo = GetLocationInfo(crossSectionSurvey);

            var crossSection = _crossSectionMapper.MapCrossSection(locationInfo, crossSectionSurvey);

            return CreateParsedResult(locationInfo, crossSection);
        }

        private ILocationInfo GetLocationInfo(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var location = GetLocationFromCrossSection(crossSectionSurvey);

            if (IsFieldVisitUpload())
                VerifyCrossSectionLocationMatchesVisitLocation(location);

            return location;
        }

        private ILocationInfo GetLocationFromCrossSection(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var crossSectionLocationIdentifier = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Location);
            var location = FindLocationByIdentifier(crossSectionLocationIdentifier);

            if (location != null)
                return location;

            throw new ParsingFailedException(
                Invariant($"Location with identifier: '{crossSectionLocationIdentifier}' does not exist"))
                {
                    UnknownLocation = crossSectionLocationIdentifier
                };
        }

        private ILocationInfo FindLocationByIdentifier(string identifier)
        {
            return _parseContext.FindLocationByIdentifier(identifier);
        }

        private bool IsFieldVisitUpload()
        {
            return _parseContext.TargetFieldVisit != null;
        }

        private void VerifyCrossSectionLocationMatchesVisitLocation(ILocationInfo fileLocation)
        {
            var targetIdentifier = _parseContext.TargetLocation.LocationIdentifier;

            if (fileLocation.LocationIdentifier.EqualsOrdinalIgnoreCase(targetIdentifier))
                return;

            throw new ParsingFailedException(
                Invariant(
                    $"Visit's location identifier '{targetIdentifier}' does not match the identifier in the file: '{fileLocation.LocationIdentifier}'"))
            {
                TargetLocation = _parseContext.TargetLocation,
                AmbiguousLocations = new[] { fileLocation }
            };
        }

        private ParsedResult CreateParsedResult(ILocationInfo locationInfo, CrossSectionSurvey crossSection)
        {
            var existingVisit = GetExistingVisit(locationInfo, crossSection);

            return existingVisit != null ?
                AddCrossSectionToExistingVisit(crossSection, existingVisit) :
                CreateNewVisit(locationInfo, crossSection);
        }

        private IFieldVisitInfo GetExistingVisit(ILocationInfo locationInfo, CrossSectionSurvey crossSectionSurvey)
        {
            if (IsFieldVisitUpload())
                return _parseContext.TargetFieldVisit;

            VerifyVisitDoesNotAlreadyExistInLocation(locationInfo, crossSectionSurvey);

            return null;
        }

        private static void VerifyVisitDoesNotAlreadyExistInLocation(ILocationInfo locationInfo,
            CrossSectionSurvey crossSectionSurvey)
        {
            var existingLocationVisits = locationInfo.FindLocationFieldVisitsInTimeRange(crossSectionSurvey.StartTime,
                crossSectionSurvey.EndTime);

            if (!existingLocationVisits.Any())
                return;

            throw new ParsingFailedException("A Visit within the time range of the Cross-Section already exists.")
            {
                TargetLocation = locationInfo,
                ConflictingVisits = existingLocationVisits.ToList()
            };
        }

        private static ParsedResult AddCrossSectionToExistingVisit(CrossSectionSurvey crossSection, IFieldVisitInfo existingVisit)
        {
            return new NewCrossSectionSurvey
            {
                FieldVisit = existingVisit,
                CrossSectionSurvey = crossSection
            };
        }

        private static NewFieldVisit CreateNewVisit(ILocationInfo locationInfo, CrossSectionSurvey crossSection)
        {
            return new NewFieldVisit
            {
                Location = locationInfo,
                FieldVisit = new BusinessInterfaces.FieldDataPlugInCore.DataModel.FieldVisit
                {
                    Party = crossSection.Party,
                    StartDate = crossSection.StartTime,
                    EndDate = crossSection.EndTime,
                    CrossSectionSurveys = new[] { crossSection }
                }
            };
        }
    }
}
