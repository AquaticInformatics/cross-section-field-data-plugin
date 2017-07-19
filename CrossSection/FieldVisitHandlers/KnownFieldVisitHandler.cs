using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class KnownFieldVisitHandler : FieldVisitHandlerBase
    {
        private FieldVisitInfo SelectedFieldVisit { get; }

        public KnownFieldVisitHandler(FieldVisitInfo selectedFieldVisit)
        {
            SelectedFieldVisit = selectedFieldVisit;
        }

        public override FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            CheckForExpectedLocation(locationIdentifier, SelectedFieldVisit.LocationInfo);

            return SelectedFieldVisit;
        }
    }
}
