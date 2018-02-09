using FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.Interfaces
{
    public interface ICrossSectionMapper
    {
        CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey);
    }
}
