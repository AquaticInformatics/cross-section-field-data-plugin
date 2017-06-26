using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.Verticals;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IVerticalMapper
    {
        List<Vertical> Map(GaugingSummaryItem gaugingSummaryItem, DeploymentMethodType deploymentMethod);
    }
}
