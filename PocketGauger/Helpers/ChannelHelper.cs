using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class ChannelHelper
    {
        public const string MainChannel = "Main";

        public static IChannelInfo GetDefaultLocationChannel(ILocationInfo locationInfo)
        {
            return locationInfo.Channels.Any() ? FindDefaultChannel(locationInfo) : CreateDefaultChannel(locationInfo);
        }

        private static IChannelInfo FindDefaultChannel(ILocationInfo locationInfo)
        {
            return GetMainChannel(locationInfo) ?? locationInfo.Channels.First();
        }

        private static IChannelInfo GetMainChannel(ILocationInfo locationInfo)
        {
            return locationInfo.Channels.FirstOrDefault(info => info.ChannelName == MainChannel);
        }

        private static IChannelInfo CreateDefaultChannel(ILocationInfo locationInfo)
        {
            return locationInfo.CreateNewChannel(MainChannel);
        }
    }
}
