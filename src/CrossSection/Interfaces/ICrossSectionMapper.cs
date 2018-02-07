using FieldDataPluginFramework.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionMapper
    {
        CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey);
    }
}
