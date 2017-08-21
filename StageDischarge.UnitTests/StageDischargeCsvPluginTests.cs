using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
/*
namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using NUnit.Framework;
    using Ploeh.AutoFixture;

    // Dougs class.
    namespace Server.Plugins.FieldVisit.StageDischargeCsv.UnitTests
    {
        [TestFixture]
        public class StageDischargeCsvPluginTests
        {
            private IFixture _fixture;
            private IFieldDataResultsAppender _mockAppender;
            private ILog _mockLogger;
            private StageDischargeCsvPlugin _plugin;

            [SetUp]
            public void ForEachTest()
            {
                _fixture = new Fixture();

                _mockAppender = Substitute.For<IFieldDataResultsAppender>();
                _mockAppender
                    .GetLocationByIdentifier(Arg.Any<string>())
                    .Returns(x => CreateMockLocation(x.Arg<string>()));

                _mockLogger = Substitute.For<ILog>();

                _plugin = new StageDischargeCsvPlugin();
            }

            private LocationInfo CreateMockLocation(string locationIdentifier)
            {
                var location = Substitute.For<LocationInfo>();

                TimeSpan offset;
                if (!_knownLocations.TryGetValue(locationIdentifier, out offset))
                {
                    offset = TimeSpan.Zero;
                }

                location.LocationIdentifier.ReturnsForAnyArgs(locationIdentifier);
                location.UtcOffset.ReturnsForAnyArgs(offset);

                return location;
            }

            private Dictionary<string, TimeSpan> _knownLocations = new Dictionary<string, TimeSpan>();

            private EmbeddedResource GetEmbeddedResource(string path)
            {
                return EmbeddedResourceLoader.GetEmbeddedResource(path);
            }

            [Test]
            public void ParseFile_Works()
            {
                var data = GetEmbeddedResource(@"TestData\TextFile1.csv");

                _knownLocations.Add("myLocation1", TimeSpan.FromHours(4));
                _knownLocations.Add("myLocation2", TimeSpan.FromHours(-7));

                var result = _plugin.ParseFile(data.Stream, _mockAppender, _mockLogger);

                result.Status.ShouldBeEquivalentTo(ParseFileStatus.ParsedSuccessfully, "{0}", result.ErrorMessage);
            }
        }
    }
}
*/
 