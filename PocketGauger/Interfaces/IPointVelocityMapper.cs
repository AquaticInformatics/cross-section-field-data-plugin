using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.ChannelMeasurements;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IPointVelocityMapper
    {
        ManualGaugingDischargeSection Map(GaugingSummaryItem summaryItem, DischargeActivity dischargeActivity);
    }
}
