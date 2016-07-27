using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionMapper
    {
        CrossSectionSurvey MapCrossSection(ILocationInfo location, Model.CrossSectionSurvey crossSectionSurvey);
    }
}
