using System.IO;

namespace CrossSectionPlugin.Interfaces
{
    public interface ICrossSectionParser
    {
        Model.CrossSectionSurvey ParseFile(Stream fileStream);
    }
}
