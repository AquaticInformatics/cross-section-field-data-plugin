using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IPointVelocityMapper
    {
        ManualGaugingDischargeSection Map(GaugingSummaryItem summaryItem, DischargeActivity dischargeActivity);
    }
}
