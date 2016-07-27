using System.Collections.Generic;
using NSubstitute;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.TestData
{
    public class TestHelpers
    {
        public static IDictionary<string, string> CreateExpectedMetadata()
        {
            return new Dictionary<string, string>
            {
                { "Location", "Server.Plugins.FieldVisit.CrossSection.Tests.CompleteCrossSection" },
                { "StartDate", "2001-05-08T14:32:15+07:00" },
                { "EndDate", "2001-05-08T17:12:45+07:00" },
                { "Party", "Cross-Section Party" },
                { "Channel", "Right overflow" },
                { "RelativeLocation", "At the Gage" },
                { "Stage", "12.2" },
                { "Unit", "ft" },
                { "StartBank", "Left bank" },
                { "Comment", "Cross-section survey comments" }
            };
        }

        public static IChannelInfo SetupMockChannel(string channelName)
        {
            var channel = Substitute.For<IChannelInfo>();
            channel.ChannelName.Returns(channelName);

            return channel;
        }

        public static IRelativeLocationInfo SetupMockRelativeLocation(string relativeLocationName)
        {
            var relativeLocation = Substitute.For<IRelativeLocationInfo>();
            relativeLocation.RelativeLocationName.Returns(relativeLocationName);

            return relativeLocation;
        }

        public static IUnit SetupMockUnit(string unitName)
        {
            var unit = Substitute.For<IUnit>();
            unit.Name.Returns(unitName);

            return unit;
        }
    }
}
