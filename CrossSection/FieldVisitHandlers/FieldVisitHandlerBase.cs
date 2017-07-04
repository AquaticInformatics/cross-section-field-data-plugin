using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Exceptions;
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

            throw new CrossSectionSurveyDataFormatException(
                Invariant($"Location identifier '{seletedLocationIdentifier}' does not match the identifier in the file: '{locationIdentifier}'"));
        }

        protected IFieldVisit CreateFieldVisit(ILocation location, CrossSectionSurvey crossSectionSurvey)
        {
            var fieldVisit = new FieldVisitDetails(crossSectionSurvey.SurveyPeriod) {Party = crossSectionSurvey.Party};
            return FieldDataResultsAppender.AddFieldVisit(location, fieldVisit);
        }
    }
}
