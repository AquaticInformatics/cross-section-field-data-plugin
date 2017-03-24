using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionMapper
    {
        CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey);
    }
}
