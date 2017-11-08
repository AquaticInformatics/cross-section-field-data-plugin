using System;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IDischargeActivityMapper
    {
        DischargeActivity Map(GaugingSummaryItem gaugingSummary, TimeSpan locationTimeZoneOffset);
    }
}
