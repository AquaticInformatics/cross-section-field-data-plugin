using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface IFieldVisitHandler
    {
        FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSection);
    }
}
