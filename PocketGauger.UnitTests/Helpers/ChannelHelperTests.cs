using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Helpers
{
    [TestFixture]
    public class ChannelHelperTests
    {
        private ILocationInfo _locationInfo;

        [SetUp]
        public void SetupForEachTest()
        {
            _locationInfo = Substitute.For<ILocationInfo>();

            _locationInfo.Channels.ReturnsForAnyArgs(new List<IChannelInfo>());

            var mainChannel = CreateMainChannel();
            _locationInfo.CreateNewChannel(Arg.Any<string>()).Returns(mainChannel);
        }

        private static IChannelInfo CreateMainChannel()
        {
            return CreateChannel(ChannelHelper.MainChannel);
        }

        private static IChannelInfo CreateChannel(string channelName)
        {
            var channel = Substitute.For<IChannelInfo>();
            channel.ChannelName.ReturnsForAnyArgs(channelName);

            return channel;
        }

        [Test]
        public void GetDefaultLocationChannel_LocationHasNoChannels_CallsCreateNewChannelOnLocation()
        {
            ChannelHelper.GetDefaultLocationChannel(_locationInfo);

            _locationInfo.Received().CreateNewChannel(ChannelHelper.MainChannel);
        }

        [Test]
        public void GetDefaultLocationChannel_LocationHasMainChannel_ReturnsMainChannel()
        {
            var mainChannel = CreateMainChannel();

            _locationInfo.Channels.ReturnsForAnyArgs(new List<IChannelInfo> { mainChannel });

            var channel =  ChannelHelper.GetDefaultLocationChannel(_locationInfo);

            Assert.That(channel, Is.EqualTo(mainChannel));
        }

        [Test]
        public void GetDefaultLocationChannel_LocationNoHasMainChannel_ReturnsFirstChannelInList()
        {
            var channels = new List<IChannelInfo>
            {
                CreateChannel("Abc"),
                CreateChannel("Other"),
                CreateChannel("Main (Left)")
            };

            _locationInfo.Channels.ReturnsForAnyArgs(channels);

            var channel = ChannelHelper.GetDefaultLocationChannel(_locationInfo);

            Assert.That(channel, Is.EqualTo(channels.First()));
        }
    }
}
