using Server.BusinessInterfaces.FieldDataPlugInCore.Results;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface IParsedResultMapper
    {
        ParsedResult CreateParsedResult(Model.CrossSectionSurvey crossSectionSurvey);
    }
}
