using System;
using System.Collections.Generic;
using NSubstitute;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.TestData
{
    public class TestHelpers
    {
        public static void RegisterMockTypes(IFixture fixture)
        {
            fixture.Register(() => SetupMockUnit(fixture.Create<string>()));
            fixture.Register(() => SetupMockChannel(fixture.Create<string>()));
            fixture.Register(() => SetupMockRelativeLocation(fixture.Create<string>()));
        }

        public static IDictionary<string, string> CreateExpectedCrossSectionFields()
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

        public static ILocationInfo SetupMockLocationInfo(string locationIdentifier)
        {
            var relativeLocation = Substitute.For<ILocationInfo>();
            relativeLocation.LocationIdentifier.Returns(locationIdentifier);

            return relativeLocation;
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
