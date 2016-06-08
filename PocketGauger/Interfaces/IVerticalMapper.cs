using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IVerticalMapper
    {
        List<Vertical> Map(GaugingSummaryItem gaugingSummaryItem, PointVelocityDischarge pointVelocityDischarge);
    }
}
