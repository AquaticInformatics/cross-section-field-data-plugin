using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionMapper
    {
        CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey);
    }
}
