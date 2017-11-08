using System.Collections.Generic;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        ICollection<ElevationMeasurement> MapPoints(List<Model.CrossSectionPoint> points);
    }
}
