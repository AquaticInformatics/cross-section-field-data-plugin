using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class KnownFieldVisitHandler : FieldVisitHandlerBase
    {
        private IFieldVisit SelectedFieldVisit { get; }

        public KnownFieldVisitHandler(IFieldVisit selectedFieldVisit)
        {
            SelectedFieldVisit = selectedFieldVisit;
        }

        public override IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            CheckForExpectedLocation(locationIdentifier, SelectedFieldVisit.Location);

            return SelectedFieldVisit;
        }
    }
}
