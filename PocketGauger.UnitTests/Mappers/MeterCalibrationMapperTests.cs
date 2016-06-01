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
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
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
        private IUnit _velocityDefaultUnit;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Register<IReadOnlyList<MeterCalibrationItem>>(() => _fixture.Create<List<MeterCalibrationItem>>());

            _parseContext = Substitute.For<IParseContext>();
            SetupVelocityParameter();
            _mapper = new MeterCalibrationMapper(_parseContext);

            _input = _fixture.Create<Dictionary<string, MeterDetailsItem>>();
        }

        private void SetupVelocityParameter()
        {
            var velocityParameter = Substitute.For<IParameter>();
            velocityParameter.Id.Returns(ParametersAndMethodsConstants.VelocityParameterId);

            _velocityDefaultUnit = Substitute.For<IUnit>();
            velocityParameter.DefaultUnit.Returns(_velocityDefaultUnit);

            _parseContext.AllParameters.ReturnsForAnyArgs(new List<IParameter> { velocityParameter });
        }

        [Test]
        public void Map_CorrectlyMapsCalibrationValues()
        {
            Action<MeterCalibration, MeterDetailsItem> comparer = (calibration, detail) =>
            {
                Assert.That(calibration.SerialNumber, Is.EqualTo(detail.MeterNumber));
                Assert.That(calibration.Model, Is.EqualTo(detail.ImpellerNumber));
                Assert.That(calibration.Configuration, Is.EqualTo(detail.Description));
                Assert.That(calibration.Manufacturer, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
            };

            VerifyCalibration(comparer);
        }

        private void VerifyCalibration(Action<MeterCalibration, MeterDetailsItem> comparer)
        {
            var result = _mapper.Map(_input);

            result.Keys.ShouldAllBeEquivalentTo(_input.Keys);
            var inputMeterDetails = _input.Values.ToList();
            var resultMeterCalibrations = result.Values.ToList();
            for (var i = 0; i < _input.Count; i++)
            {
                comparer(resultMeterCalibrations[i], inputMeterDetails[i]);
            }
        }

        [TestCase(MeterType.ElectroMagneticCurrentMeter, FieldDataMeterType.ElectromagneticVelocityMeter)]
        [TestCase(MeterType.RotatingElementCurrentMeter, FieldDataMeterType.PriceAa)]
        [TestCase(MeterType.AcousticDopplerCurrentProfiler, FieldDataMeterType.Adcp)]
        [TestCase(null, FieldDataMeterType.Unspecified)]
        public void Map_SetsCorrectMeterType(MeterType? inputMeterType, FieldDataMeterType expectedResultMeterType)
        {
            var meterDetailsItem = _fixture.Build<MeterDetailsItem>().With(m => m.MeterType, inputMeterType).Create();
            _input = new Dictionary<string, MeterDetailsItem>
            {
                {_fixture.Create<string>(), meterDetailsItem}
            };
            Action<MeterCalibration, MeterDetailsItem> comparer = (calibration, detail) =>
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
                            DoubleHelper.AreEqual(e.Intercept, calibration.Constant) && 
                            e.InterceptUnit == _velocityDefaultUnit);
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
        public void Map_SetsLastEquationRangeEndToRangeStartPlus100()
        {
            var result = _mapper.Map(_input);

            var lastEquationsForEachCalibration = result.Values.Select(v => v.Equations.Last());
            Assert.That(lastEquationsForEachCalibration,
                Has.All.Matches<MeterCalibrationEquation>(e => e.RangeEnd == e.RangeStart + 100));
        }

        [Test]
        public void Map_CalibrationMinRotationSpeedIsNull_EquationRangeStartAndRangeEndAreSetToNull()
        {
            _input.Values.ToList().SelectMany(m => m.Calibrations).ToList().ForEach(c => c.MinRotationSpeed = null);

            var result = _mapper.Map(_input);

            var equations = result.Values.SelectMany(c => c.Equations);
            Assert.That(equations,
                Has.All.Matches<MeterCalibrationEquation>(e => e.RangeStart == null && e.RangeEnd == null));
        }
    }
}
