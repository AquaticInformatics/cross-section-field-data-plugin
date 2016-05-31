using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using MeterCalibration = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterCalibration;
using FieldDataMeterType = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterType;
using MeterType = Server.Plugins.FieldVisit.PocketGauger.Dtos.MeterType;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    public class MeterCalibrationMapperTests
    {
        private IFixture _fixture;

        private IParseContext _parseContext;
        private MeterCalibrationMapper _mapper;

        private IReadOnlyDictionary<string, MeterDetailsItem> _input;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Register<IReadOnlyList<MeterCalibrationItem>>(() => _fixture.Create<List<MeterCalibrationItem>>());

            _parseContext = Substitute.For<IParseContext>();
            _mapper = new MeterCalibrationMapper(_parseContext);

            _input = _fixture.Create<Dictionary<string, MeterDetailsItem>>();
        }

        [Test]
        public void Map_CorrectlyMapsCalibrationValues()
        {
            Action<MeterDetailsItem, MeterCalibration> comparer = (detail, calibration) =>
            {
                Assert.That(detail.MeterNumber, Is.EqualTo(calibration.SerialNumber));
                Assert.That(detail.ImpellerNumber, Is.EqualTo(calibration.Model));
                Assert.That(detail.Description, Is.EqualTo(calibration.Configuration));
            };

            VerifyCalibration(comparer);
        }

        private void VerifyCalibration(Action<MeterDetailsItem, MeterCalibration> comparer)
        {
            var result = _mapper.Map(_input);

            result.Keys.ShouldAllBeEquivalentTo(_input.Keys);
            var inputMeterDetails = _input.Values.ToList();
            var resultMeterCalibrations = result.Values.ToList();
            for (var i = 0; i < _input.Count; i++)
            {
                comparer(inputMeterDetails[i], resultMeterCalibrations[i]);
            }
        }

        [Test]
        public void Map_SetsCorrectPropertyValuesToNonApplicable()
        {
            Action<MeterDetailsItem, MeterCalibration> comparer = (detail, calibration) =>
            {
                Assert.That(calibration.Manufacturer, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
                Assert.That(calibration.FirmwareVersion, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
                Assert.That(calibration.SoftwareVersion, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
            };

            VerifyCalibration(comparer);
        }

        [TestCase(MeterType.ElectroMagneticCurrentMeter, FieldDataMeterType.ElectromagneticVelocityMeter)]
        [TestCase(MeterType.RotatingElementCurrentMeter, FieldDataMeterType.PriceAa)]
        [TestCase(MeterType.AcousticDopplerCurrentProfiler, FieldDataMeterType.Adcp)]
        [TestCase(null, FieldDataMeterType.Unknown)]
        public void Map_SetsCorrectMeterType(MeterType? inputMeterType, FieldDataMeterType expectedResultMeterType)
        {
            var meterDetailsItem = _fixture.Build<MeterDetailsItem>().With(m => m.MeterType, inputMeterType).Create();
            _input = new Dictionary<string, MeterDetailsItem>
            {
                {_fixture.Create<string>(), meterDetailsItem}
            };
            Action<MeterDetailsItem, MeterCalibration> comparer = (detail, calibration) =>
            {
                Assert.That(calibration.MeterType, Is.EqualTo(expectedResultMeterType));
            };

            VerifyCalibration(comparer);
        }

        [Test]
        public void Map_CorrectlyMapsEquationValues()
        {
            var result = _mapper.Map(_input);

            var inputCalibrations = _input.Values.SelectMany(i => i.Calibrations).ToList();
            var resultEquations = result.Values.SelectMany(r => r.Equations).ToList();
            foreach (var calibration in inputCalibrations)
            {
                resultEquations.Should()
                    .ContainSingle(
                        e =>
                            e.RangeStart == calibration.MinRotationSpeed &&
                            DoubleHelper.AreEqual(e.Slope, calibration.Factor) &&
                            DoubleHelper.AreEqual(e.Intercept, calibration.Constant));
            }
        }

        [Test]
        public void Map_OrdersEquationsByRangeStart()
        {
            var result = _mapper.Map(_input);

            foreach (var meterCalibration in result.Values)
            {
                Assert.That(meterCalibration.Equations, Is.Ordered.By("RangeStart"));
            }
        }

        [Test]
        public void Map_SetsAllButLastEquationRangeEndToRangeStartOfNextEquation()
        {
            var result = _mapper.Map(_input);

            var resultValues = result.Values.ToList();
            foreach (var calibration in resultValues)
            {
                var equations = calibration.Equations.ToList();
                for (var i = 0; i < calibration.Equations.Count - 1; i++)
                {
                    Assert.That(equations[i].RangeEnd, Is.EqualTo(equations[i + 1].RangeStart));
                }
            }
        }

        [Test]
        public void Map_SetsLastEquationRangeEndToNull()
        {
            var result = _mapper.Map(_input);

            var lastEquationsForEachCalibration = result.Values.Select(v => v.Equations.Last());
            Assert.That(lastEquationsForEachCalibration,
                Has.All.Matches<MeterCalibrationEquation>(e => e.RangeEnd == null));
        }
    }
}
