using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface IFieldVisitHandler
    {
        FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSection);
    }
}
