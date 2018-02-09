using System.Collections.Generic;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        ICollection<CrossSectionPoint> MapPoints(List<Model.CrossSectionPoint> points);
    }
}
