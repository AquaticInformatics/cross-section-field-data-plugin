using System.Collections.Generic;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IVerticalMapper
    {
        List<Vertical> Map(GaugingSummaryItem gaugingSummaryItem, DeploymentMethodType deploymentMethod);
    }
}
