using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class KnownLocationHandler : FieldVisitHandlerBase
    {
        private ILocation SelectedLocation { get; }

        public KnownLocationHandler(ILocation selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender)
            : base(fieldDataResultsAppender)
        {
            SelectedLocation = selectedLocation;
        }

        public override IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            CheckForExpectedLocation(locationIdentifier, SelectedLocation);

            return CreateFieldVisit(SelectedLocation, crossSectionSurvey);
        }
    }
}
