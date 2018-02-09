using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.Interfaces
{
    public interface IFieldVisitHandler
    {
        FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSection);
    }
}
