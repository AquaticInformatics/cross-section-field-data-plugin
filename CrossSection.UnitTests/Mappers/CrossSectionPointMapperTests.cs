using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using PluginFramework = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Mappers
{
    [TestFixture]
    public class CrossSectionPointMapperTests
    {
        private IFixture _fixture;
        private ICrossSectionPointMapper _crossSectionPointMapper;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _fixture = new Fixture();
            _crossSectionPointMapper = new CrossSectionPointMapper();
        }

        [Test]
        public void MapPoints_CrossSectionsPoints_IsMappedAsExpected()
        {
            var points = _fixture.CreateMany<CrossSectionPoint>().ToList();

            var actual = _crossSectionPointMapper.MapPoints(points);

            AssertPointsMatchExpected(actual, points);
        }

        private static void AssertPointsMatchExpected(IEnumerable<PluginFramework.CrossSectionPoint> actualPoints,
            IEnumerable<CrossSectionPoint> expectedPoints)
        {
            foreach (var point in actualPoints.Zip(expectedPoints, Tuple.Create))
            {
                var actual = point.Item1;
                var expectation = point.Item2;

                AssertPointIsEqual(actual, expectation);
            }
        }

        private static void AssertPointIsEqual(PluginFramework.CrossSectionPoint actual, CrossSectionPoint expectation)
        {
            Assert.That(actual.Distance, Is.EqualTo(expectation.Distance));
            Assert.That(actual.Elevation, Is.EqualTo(expectation.Elevation));
            Assert.That(actual.Comments, Is.EqualTo(expectation.Comment));
            Assert.That(actual.Depth, Is.Null);
        }

        [Test]
        public void MapPoints_EmptyCrossSectionsPoints_EmptyPointsAreIgnored()
        {
            var points = new List<CrossSectionPoint> { CreateEmptyPoint(), CreateEmptyPoint(), null };

            var actual = _crossSectionPointMapper.MapPoints(points);

            Assert.That(actual, Is.Empty);
        }

        private static CrossSectionPoint CreateEmptyPoint()
        {
            return new CrossSectionPoint
            {
                Distance = null,
                Elevation = null,
                Comment = null
            };
        }

        [Test]
        public void MapPoints_InvalidPoint_Throws()
        {
            var points = new List<CrossSectionPoint> { CreateInvalidPoint() };

            TestDelegate testDelegate = () => _crossSectionPointMapper.MapPoints(points);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>()
                .With.Message.Contains("must have both a Distance and Elevation"));
        }

        private CrossSectionPoint CreateInvalidPoint()
        {
            return _fixture.Build<CrossSectionPoint>()
                .With(point => point.Distance, default(double?))
                .With(point => point.Elevation, default(double?))
                .Create();
        }
    }
}
