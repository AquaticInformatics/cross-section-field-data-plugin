using System;
using System.Collections.Generic;
using System.Linq;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Interfaces;
using CrossSectionPlugin.Mappers;
using CrossSectionPlugin.Model;
using CrossSectionPlugin.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Framework = FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.UnitTests.Mappers
{
    [TestFixture]
    public class CrossSectionPointMapperTests
    {
        private IFixture Fixture { get; set; }
        private ICrossSectionPointMapper CrossSectionPointMapper { get; set; }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            Fixture = new Fixture();
            CrossSectionPointMapper = new CrossSectionPointMapper();
        }

        [Test]
        public void MapPoints_NullPointsCollection_Throws()
        {
            void MapPointsWithNullPointsCollection() => CrossSectionPointMapper.MapPoints(null);

            Assert.That(MapPointsWithNullPointsCollection, Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void MapPoints_V1CrossSectionsPoints_IsMappedAsExpected()
        {
            var points = CreateV1Points();

            var actual = CrossSectionPointMapper.MapPoints(points);

            AssertV1PointsMatchExpected(actual, points);
        }

        private List<ICrossSectionPoint> CreateV1Points()
        {
            return new List<ICrossSectionPoint>(Fixture.CreateMany<CrossSectionPointV1>()
                .OrderBy(p => p.Distance)
                .ToList());
        }

        [Test]
        public void MapPoints_EmptyV1CrossSectionsPoints_EmptyPointsAreIgnored()
        {
            var points = new List<ICrossSectionPoint> { new CrossSectionPointV1(), null };

            var actual = CrossSectionPointMapper.MapPoints(points);

            Assert.That(actual, Is.Empty);
        }

        private void AssertV1PointsMatchExpected(List<Framework.CrossSectionPoint> actualPoints,
            List<ICrossSectionPoint> expectedPoints)
        {
            actualPoints.Should().HaveCount(expectedPoints.Count);
            for (var i = 0; i < actualPoints.Count; i++)
            {
                var actual = actualPoints[i];
                var expected = expectedPoints[i] as CrossSectionPointV1;

                Assert.That(expected, Is.Not.Null);
                actual.PointOrder.Should().Be(i + 1);
                actual.Distance.Should().Be(expected.Distance);
                actual.Elevation.Should().Be(expected.Elevation);
                actual.Comments.Should().Be(expected.Comment);
                actual.Depth.Should().Be(null);
            }
        }

        [Test]
        public void MapPoints_V1PointWithoutDistanceAndElevation_Throws()
        {
            var points = new List<ICrossSectionPoint> { CreateV1PointWithoutDistanceAndElevation() };

            void MapPointsWithoutRequiredFields() => CrossSectionPointMapper.MapPoints(points);

            Assert.That(MapPointsWithoutRequiredFields, Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>()
                .With.Message.Contains("must have both a Distance and Elevation"));
        }

        private CrossSectionPointV1 CreateV1PointWithoutDistanceAndElevation()
        {
            return Fixture.Build<CrossSectionPointV1>()
                .With(point => point.Distance, null)
                .With(point => point.Elevation, null)
                .Create();
        }

        [Test]
        public void MapPoints_V2CrossSectionsPoints_IsMappedAsExpected()
        {
            var points = CreateV2Points();

            var actual = CrossSectionPointMapper.MapPoints(points);

            AssertV2PointsMatchExpected(actual, points);
        }

        private List<ICrossSectionPoint> CreateV2Points()
        {
            return new List<ICrossSectionPoint>(Fixture.CreateMany<CrossSectionPointV2>()
                .OrderBy(p => p.PointOrder)
                .ToList());
        }

        private void AssertV2PointsMatchExpected(List<Framework.CrossSectionPoint> actualPoints,
            List<ICrossSectionPoint> expectedPoints)
        {
            actualPoints.Should().HaveCount(expectedPoints.Count);
            for (var i = 0; i < actualPoints.Count; i++)
            {
                var actual = actualPoints[i];
                var expected = expectedPoints[i] as CrossSectionPointV2;

                Assert.That(expected, Is.Not.Null);
                actual.PointOrder.Should().Be(expected.PointOrder);
                actual.Distance.Should().Be(expected.Distance);
                actual.Elevation.Should().Be(expected.Elevation);
                actual.Comments.Should().Be(expected.Comment);
                actual.Depth.Should().Be(null);
            }
        }

        [Test]
        public void MapPoints_EmptyV2CrossSectionsPoints_EmptyPointsAreIgnored()
        {
            var points = new List<ICrossSectionPoint> { new CrossSectionPointV2(), null };

            var actual = CrossSectionPointMapper.MapPoints(points);

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void MapPoints_V2PointWithoutPointOrderDistanceAndElevation_Throws()
        {
            var points = new List<ICrossSectionPoint> { CreateV2PointWithoutPointOrderDistanceAndElevation() };

            void MapPointsWithoutRequiredFields() => CrossSectionPointMapper.MapPoints(points);

            Assert.That(MapPointsWithoutRequiredFields, Throws.Exception.TypeOf<CrossSectionSurveyDataFormatException>()
                .With.Message.Contains("must have PointOrder, Distance and Elevation"));
        }

        private CrossSectionPointV2 CreateV2PointWithoutPointOrderDistanceAndElevation()
        {
            return Fixture.Build<CrossSectionPointV2>()
                .Without(point => point.PointOrder)
                .Without(point => point.Distance)
                .Without(point => point.Elevation)
                .Create();
        }

        [Test]
        public void MapPoints_MixOfCrossSectionPointVersions_Throws()
        {
            var points = CreateV1Points()
                .Concat(CreateV2Points())
                .ToList();

            void MapMixedPointsVersions() => CrossSectionPointMapper.MapPoints(points);

            Assert.That(MapMixedPointsVersions, Throws.Exception.TypeOf<InvalidCastException>());
        }

        [Test]
        public void MapPoints_UnknownICrossSectionPoint_Throws()
        {
            var points = new List<ICrossSectionPoint> { new TestCrossSectionPoint() };

            void MapMixedPointsVersions() => CrossSectionPointMapper.MapPoints(points);

            Assert.That(MapMixedPointsVersions, Throws.Exception.TypeOf<CrossSectionCsvFormatException>()
                .With.Message.Contains("Unsupported Cross-Section type"));
        }
    }
}
