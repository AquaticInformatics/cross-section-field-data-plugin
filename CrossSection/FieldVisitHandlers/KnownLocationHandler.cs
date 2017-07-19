using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class KnownLocationHandler : FieldVisitHandlerBase
    {
        private LocationInfo SelectedLocation { get; }

        public KnownLocationHandler(LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender)
            : base(fieldDataResultsAppender)
        {
            SelectedLocation = selectedLocation;
        }

        public override FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            CheckForExpectedLocation(locationIdentifier, SelectedLocation);

            return CreateFieldVisit(SelectedLocation, crossSectionSurvey);
        }
    }
}
