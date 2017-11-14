using System.Collections.Generic;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        ICollection<CrossSectionPoint> MapPoints(List<Model.CrossSectionPoint> points);
    }
}
