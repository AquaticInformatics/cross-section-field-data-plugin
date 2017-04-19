using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;

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
