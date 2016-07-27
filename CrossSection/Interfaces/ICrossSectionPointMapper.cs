using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.Interfaces
{
    public interface ICrossSectionPointMapper
    {
        ICollection<CrossSectionPoint> MapPoints(List<Model.CrossSectionPoint> points);
    }
}
