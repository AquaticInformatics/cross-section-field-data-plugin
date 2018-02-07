using System.IO;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionParser
    {
        Model.CrossSectionSurvey ParseFile(Stream fileStream);
    }
}
