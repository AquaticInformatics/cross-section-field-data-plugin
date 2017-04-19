using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public abstract class FieldVisitHandlerBase : IFieldVisitHandler
    {
        protected IFieldDataResultsAppender FieldDataResultsAppender { get; }

        protected FieldVisitHandlerBase(IFieldDataResultsAppender fieldDataResultsAppender)
        {
            FieldDataResultsAppender = fieldDataResultsAppender;
        }

        protected FieldVisitHandlerBase()
        {
        }

        public abstract IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey);

        protected static void CheckForExpectedLocation(string locationIdentifier, ILocation selectedLocation)
        {
            var seletedLocationIdentifier = selectedLocation.LocationIdentifier;

            if (locationIdentifier.EqualsOrdinalIgnoreCase(seletedLocationIdentifier))
                return;

            throw new ParsingFailedException(
                Invariant($"Location identifier '{seletedLocationIdentifier}' does not match the identifier in the file: '{locationIdentifier}'"));
        }

        protected IFieldVisit CreateFieldVisit(ILocation location, CrossSectionSurvey crossSectionSurvey)
        {
            var fieldVisit = new FieldVisitDetails(crossSectionSurvey.SurveyPeriod, crossSectionSurvey.Party);
            return FieldDataResultsAppender.AddFieldVisit(location, fieldVisit);
        }
    }
}
