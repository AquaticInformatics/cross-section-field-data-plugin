using System.Collections.Generic;
using CrossSectionPlugin.Model;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        List<CrossSectionPoint> MapPoints(List<ICrossSectionPoint> points);
    }
}
