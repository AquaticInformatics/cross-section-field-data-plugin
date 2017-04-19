using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface IFieldVisitHandler
    {
        IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSection);
    }
}
