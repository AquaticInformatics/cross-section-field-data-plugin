using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class KnownFieldVisitHandler : FieldVisitHandlerBase
    {
        private NewFieldVisitInfo SelectedFieldVisit { get; }

        public KnownFieldVisitHandler(NewFieldVisitInfo selectedFieldVisit)
        {
            SelectedFieldVisit = selectedFieldVisit;
        }

        public override NewFieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            CheckForExpectedLocation(locationIdentifier, SelectedFieldVisit.LocationInfo);

            return SelectedFieldVisit;
        }
    }
}
