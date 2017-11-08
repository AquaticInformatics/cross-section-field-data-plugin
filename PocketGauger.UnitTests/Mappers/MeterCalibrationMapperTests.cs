using System.Linq;
using FieldDataPluginFramework.DataModel.Meters;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;
using DoubleHelper = Common.Utils.DoubleHelper;
using FieldDataMeterType = FieldDataPluginFramework.DataModel.Meters.MeterType;
using MeterType = Server.Plugins.FieldVisit.PocketGauger.Dtos.MeterType;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    public class MeterCalibrationMapperTests
    {
        private IFixture _fixture;

        private MeterCalibrationMapper _mapper;

        private MeterDetailsItem _input;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            CollectionRegistrar.Register(_fixture);

            _mapper = new MeterCalibrationMapper();

            _input = _fixture.Create<MeterDetailsItem>();
        }

        [Test]
        public void Map_CorrectlyMapsCalibrationValues()
        {
            var result = _mapper.Map(_input);

            Assert.That(result.SerialNumber, Is.EqualTo(_input.MeterNumber));
            Assert.That(result.Model, Is.EqualTo(_input.ImpellerNumber));
            Assert.That(result.Configuration, Is.EqualTo(_input.Description));
            Assert.That(result.Manufacturer, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
        }

        [TestCase(MeterType.ElectroMagneticCurrentMeter, FieldDataMeterType.ElectromagneticVelocityMeter)]
        [TestCase(MeterType.RotatingElementCurrentMeter, FieldDataMeterType.PriceAa)]
        [TestCase(MeterType.AcousticDopplerCurrentProfiler, FieldDataMeterType.Adcp)]
        [TestCase(null, FieldDataMeterType.Unspecified)]
        public void Map_SetsCorrectMeterType(MeterType? inputMeterType, FieldDataMeterType expectedResultMeterType)
        {
            var meterDetailsItem = _fixture.Build<MeterDetailsItem>().With(m => m.MeterType, inputMeterType).Create();

            var result = _mapper.Map(meterDetailsItem);

            Assert.That(result.MeterType, Is.EqualTo(expectedResultMeterType));
        }

        [Test]
        public void Map_CorrectlyMapsEquationValues()
        {
            var result = _mapper.Map(_input);

            var inputCalibration = _input.Calibrations;
            var resultEquations = result.Equations;
            foreach (var calibration in inputCalibration)
            {
                resultEquations.Should()
                    .ContainSingle(
                        e =>
                            e.RangeStart == calibration.MinRotationSpeed &&
                            DoubleHelper.AreEqual(e.Slope, calibration.Factor.GetValueOrDefault()) &&
                            DoubleHelper.AreEqual(e.Intercept, calibration.Constant.GetValueOrDefault()) &&
                            e.InterceptUnitId == ParametersAndMethodsHelper.UnitSystem.VelocityUnitId);
            }
        }

        [Test]
        public void Map_OrdersEquationsByRangeStart()
        {
            var result = _mapper.Map(_input);

            Assert.That(result.Equations, Is.Ordered.By("RangeStart"));
        }

        [Test]
        public void Map_SetsAllButLastEquationRangeEndToRangeStartOfNextEquation()
        {
            var result = _mapper.Map(_input);

            var equations = result.Equations.ToList();
            for (var i = 0; i < equations.Count - 1; i++)
            {
                Assert.That(equations[i].RangeEnd, Is.EqualTo(equations[i + 1].RangeStart));
            }
        }

        [Test]
        public void Map_SetsLastEquationRangeEndToRangeStartPlus100()
        {
            var result = _mapper.Map(_input);

            var lastEquation = result.Equations.Last();
            Assert.That(lastEquation.RangeEnd, Is.EqualTo(lastEquation.RangeStart + 100));
        }

        [Test]
        public void Map_CalibrationMinRotationSpeedIsNull_EquationRangeStartAndRangeEndAreSetToNull()
        {
            _input.Calibrations.ToList().ForEach(c => c.MinRotationSpeed = null);

            var result = _mapper.Map(_input);

            Assert.That(result.Equations,
                Has.All.Matches<MeterCalibrationEquation>(e => e.RangeStart == null && e.RangeEnd == null));
        }

        [Test]
        public void Map_ImpellerNumberIsNull_ModelIsSetToNonApplicable()
        {
            _input.ImpellerNumber = null;

            var result = _mapper.Map(_input);

            Assert.That(result.Model, Is.EqualTo(MeterCalibrationMapper.NonApplicable));
        }
    }
}
