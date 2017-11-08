using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using FieldDataPluginFramework.DataModel.Verticals;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;
using MeterCalibration = FieldDataPluginFramework.DataModel.Meters.MeterCalibration;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    [TestFixture]
    public class VerticalMapperTests
    {
        private IFixture _fixture;

        private VerticalMapper _verticalMapper;
        private IMeterCalibrationMapper _meterCalibrationMapper;

        private GaugingSummaryItem _gaugingSummaryItem;
        private DeploymentMethodType _deploymentMethod;

        [TestFixtureSetUp]
        public void SetUp()
        {
            SetupAutoFixture();
            SetUpMeterCalibrationMapper();

            _gaugingSummaryItem = _fixture.Create<GaugingSummaryItem>();
            _deploymentMethod = _fixture.Create<DeploymentMethodType>();

            _verticalMapper = new VerticalMapper(_meterCalibrationMapper);
        }

        private void SetupAutoFixture()
        {
            _fixture = new Fixture();
            CollectionRegistrar.Register(_fixture);
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
        }

        private void SetUpMeterCalibrationMapper()
        {
            _meterCalibrationMapper = Substitute.For<IMeterCalibrationMapper>();
            var meterCalibration = _fixture.Build<MeterCalibration>().Create();
            _meterCalibrationMapper.Map(Arg.Any<MeterDetailsItem>()).Returns(meterCalibration);
        }

        [Test]
        public void Map_SetsCorrectVerticalProperties()
        {
            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            var inputPanels = _gaugingSummaryItem.PanelItems.ToList();
            for (var i = 0; i < inputPanels.Count; i++)
            {
                Assert.That(result[i].SequenceNumber, Is.EqualTo(inputPanels[i].VerticalNumber));
                Assert.That(result[i].MeasurementConditionData, Is.InstanceOf<OpenWaterData>());
                Assert.That(result[i].FlowDirection, Is.EqualTo(FlowDirectionType.Normal));
                Assert.That(result[i].TaglinePosition, Is.EqualTo(inputPanels[i].Distance));
                Assert.That(result[i].SoundedDepth, Is.EqualTo(inputPanels[i].Depth));
                Assert.That(result[i].IsSoundedDepthEstimated, Is.False);
                Assert.That(result[i].EffectiveDepth, Is.EqualTo(inputPanels[i].Depth));
            }
        }

        [Test]
        public void Map_SetsCorrectVerticalType()
        {
            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            Assert.That(result.First().VerticalType == VerticalType.StartEdgeNoWaterBefore);
            var midRiverVerticals = result.Skip(1).Take(result.Count - 2);
            Assert.That(midRiverVerticals, Has.All.Matches<Vertical>(v => v.VerticalType == VerticalType.MidRiver));
            Assert.That(result.Last().VerticalType == VerticalType.EndEdgeNoWaterAfter);
        }

        [Test]
        public void Map_SetsCorrectSegmentProperties()
        {
            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            var inputPanels = _gaugingSummaryItem.PanelItems.ToList();
            for (var i = 0; i < inputPanels.Count; i++)
            {
                Assert.That(result[i].Segment.Area, Is.EqualTo(inputPanels[i].Area));
                Assert.That(result[i].Segment.Velocity, Is.EqualTo(inputPanels[i].MeanVelocity));
                Assert.That(result[i].Segment.Discharge, Is.EqualTo(inputPanels[i].Flow));
                Assert.That(result[i].Segment.IsDischargeEstimated, Is.False);
            }
        }

        private static readonly List<Tuple<double, double, double>> GetSegmentWidthTestCases =
            new List<Tuple<double, double, double>>
            {
                Tuple.Create(6d, 3d, 2d),
                Tuple.Create(0.129, 0.41, 0.3146341463414634),
                Tuple.Create(0.033, 0.12, 0.275),
                Tuple.Create(60.1111, 0d, 0d)
            };

        [TestCaseSource(nameof(GetSegmentWidthTestCases))]
        public void Map_CalculatesCorrectSegmentWidth(Tuple<double, double, double> testData)
        {
            var area = testData.Item1;
            var depth = testData.Item2;
            var expectedWidth = testData.Item3;

            var panelItem = _gaugingSummaryItem.PanelItems.First();
            panelItem.Area = area;
            panelItem.Depth = depth;

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);
            var firstVertical = result.First();

            Assert.That(DoubleHelper.AreEqual(firstVertical.Segment.Width, expectedWidth));
        }

        [Test]
        public void Map_SetsCorrectVelocityObservationProperties()
        {
            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            for (var i = 0; i < _gaugingSummaryItem.PanelItems.Count; i++)
            {
                Assert.That(result[i].VelocityObservation.DeploymentMethod,
                    Is.EqualTo(_deploymentMethod));
                Assert.That(result[i].VelocityObservation.MeanVelocity,
                    Is.EqualTo(_gaugingSummaryItem.PanelItems.ToList()[i].MeanVelocity));
            }
        }

        [Test]
        public void Map_RetrievesMeterCalibrationMapperFromMapper()
        {
            _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            _meterCalibrationMapper.Received().Map(_gaugingSummaryItem.MeterDetailsItem);
        }

        [Test]
        public void Map_SetsCorrectVelocityDepthObservationProperties()
        {
            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            var panelItems = _gaugingSummaryItem.PanelItems.ToList();
            for (var i = 0; i < panelItems.Count; i++)
            {
                var verticalItems = panelItems[i].Verticals;
                for (var j = 0; j < verticalItems.Count; j++)
                {
                    var resultObservation = result[i].VelocityObservation.Observations.ToList()[j];

                    Assert.That(resultObservation.RevolutionCount, Is.EqualTo((int?)verticalItems[j].Revs));
                    Assert.That(resultObservation.ObservationInterval, Is.EqualTo(verticalItems[j].ExposureTime));
                    Assert.That(resultObservation.Velocity, Is.EqualTo(verticalItems[j].Velocity));
                }
            }
        }

        [Test]
        public void Map_VelocityObservationDepth_IsCalculatedAsExpected()
        {
            const double sampleDepth = 0.6;
            const double depthValue = 15;
            const double expectedObservationDepth = 9;

            var verticals = _fixture.Build<VerticalItem>()
                .With(item => item.SamplePosition, sampleDepth)
                .With(item => item.Depth, depthValue)
                .CreateMany(1)
                .ToList();

            _gaugingSummaryItem.PanelItems.First().Verticals = verticals;

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            var velocityDepthObservation = result.First().VelocityObservation.Observations.First();
            Assert.That(velocityDepthObservation.Depth, Is.EqualTo(expectedObservationDepth));
        }

        private static readonly List<Tuple<int, PointVelocityObservationType>> MultipleVelocityObservationTestCases =
            new List<Tuple<int, PointVelocityObservationType>>
            {
                Tuple.Create(2, PointVelocityObservationType.OneAtPointTwoAndPointEight),
                Tuple.Create(3, PointVelocityObservationType.OneAtPointTwoPointSixAndPointEight),
                Tuple.Create(5, PointVelocityObservationType.FivePoint),
                Tuple.Create(6, PointVelocityObservationType.SixPoint),
                Tuple.Create(11, PointVelocityObservationType.ElevenPoint)
            };

        [TestCaseSource(nameof(MultipleVelocityObservationTestCases))]
        public void Map_VerticalWithExpectedNumberOfObservations_SetsExpectedVelocityObservationType(
            Tuple<int, PointVelocityObservationType> testData)
        {
            var velocityObservationCount = testData.Item1;
            var expectedVelocityObservationMethod = testData.Item2;
            _gaugingSummaryItem.PanelItems.First().Verticals = _fixture.CreateMany<VerticalItem>(velocityObservationCount).ToList();

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            AssertVelocityObservationMethodIsExpected(result, expectedVelocityObservationMethod);
        }

        private static void AssertVelocityObservationMethodIsExpected(IEnumerable<Vertical> result,
            PointVelocityObservationType? expectedVelocityObservationMethod)
        {
            var velocityObservationMethod = result.First().VelocityObservation.VelocityObservationMethod;

            Assert.That(velocityObservationMethod, Is.EqualTo(expectedVelocityObservationMethod));
        }

        [Test]
        public void Map_UnknownVelocityObservationCount_SetsVelocityObservationTypeToNull()
        {
            const int unknownObservationCount = 15;

            _gaugingSummaryItem.PanelItems.First().Verticals = _fixture.CreateMany<VerticalItem>(unknownObservationCount).ToList();

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            AssertVelocityObservationMethodIsExpected(result, null);
        }

        private static readonly List<Tuple<double, PointVelocityObservationType>> DepthValueToVelocityObservationTestCases =
            new List<Tuple<double, PointVelocityObservationType>>
            {
                Tuple.Create(0.5, PointVelocityObservationType.OneAtPointFive),
                Tuple.Create(0.6, PointVelocityObservationType.OneAtPointSix)
            };

        [TestCaseSource(nameof(DepthValueToVelocityObservationTestCases))]
        public void Map_VerticalWithSingleObservation_SetsExpectedVelocityObservationType(
            Tuple<double, PointVelocityObservationType> testData)
        {
            var observationDepth = testData.Item1;
            var expectedVelocityObservationMethod = testData.Item2;
            var verticals = _fixture.Build<VerticalItem>()
                .With(item => item.SamplePosition, observationDepth)
                .CreateMany(1)
                .ToList();

            _gaugingSummaryItem.PanelItems.First().Verticals = verticals;

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            AssertVelocityObservationMethodIsExpected(result, expectedVelocityObservationMethod);
        }

        [Test]
        public void Map_VerticalWithSingleObservationWithNonPoint5Or6SampleDepth_SetsVelocityObservationTypeToSurface()
        {
            const double nonPointFiveOrSixDepth = 10;

            var verticals = _fixture.Build<VerticalItem>()
                .With(item => item.SamplePosition, nonPointFiveOrSixDepth)
                .CreateMany(1)
                .ToList();

            _gaugingSummaryItem.PanelItems.First().Verticals = verticals;

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            AssertVelocityObservationMethodIsExpected(result, PointVelocityObservationType.Surface);
        }

        private static readonly List<Tuple<double[], double[]>> GetTotalDischargePortionTestCases =
            new List<Tuple<double[], double[]>>
            {
                Tuple.Create(new double[] {2, 3, 4}, new[] {22.222222222222221, 33.333333333333329, 44.444444444444443}),
                Tuple.Create(new double[] {400, 600, 1100}, new[] {19.047619047619047, 28.571428571428569, 52.380952380952387}),
                Tuple.Create(new[] {50.23, 55.5, 60.1111}, new[] {30.288028721468923, 33.465769341857964, 36.246201936673124})
            };

        [TestCaseSource(nameof(GetTotalDischargePortionTestCases))]
        public void Map_CalculatesCorrectTotalDischargePortionValues(Tuple<double[], double[]> testData)
        {
            var inputDischargeValues = testData.Item1;
            var panelItems = _gaugingSummaryItem.PanelItems.ToList();
            for (var i = 0; i < panelItems.Count; i++)
            {
                panelItems[i].Flow = inputDischargeValues[i];
            }

            var result = _verticalMapper.Map(_gaugingSummaryItem, _deploymentMethod);

            var expectedTotalDischargePortionValues = testData.Item2;
            for (var i = 0; i < result.Count; i++)
            {
                Assert.That(DoubleHelper.AreEqual(result[i].Segment.TotalDischargePortion,
                    expectedTotalDischargePortionValues[i]));
            }
        }
    }
}
