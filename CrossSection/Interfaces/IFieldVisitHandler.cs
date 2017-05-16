using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface IFieldVisitHandler
    {
        IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSection);
    }
}
