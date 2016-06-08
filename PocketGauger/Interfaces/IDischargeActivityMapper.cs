using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IDischargeActivityMapper
    {
        DischargeActivity Map(ILocationInfo locationInfo, GaugingSummaryItem gaugingSummary);
    }
}
