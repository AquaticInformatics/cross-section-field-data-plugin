using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        ICollection<ElevationMeasurement> MapPoints(List<Model.CrossSectionPoint> points);
    }
}
