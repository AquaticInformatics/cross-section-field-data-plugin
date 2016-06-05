using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using MeterCalibration = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterCalibration;
using MeterType = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterType;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class MeterCalibrationMapper : IMeterCalibrationMapper
    {
        public static string NonApplicable = "N/A";
        private readonly IParseContext _parseContext;

        public MeterCalibrationMapper(IParseContext parseContext)
        {
            _parseContext = parseContext;
        }

        public MeterCalibration Map(MeterDetailsItem meterDetailsItem)
        {
            var calibration = CreateCalibration(meterDetailsItem);
            calibration.Equations = CreateCalibrationEquations(meterDetailsItem).ToList();

            return calibration;
        }

        private MeterCalibration CreateCalibration(MeterDetailsItem meterDetailsItem)
        {
            return new MeterCalibration
            {
                SerialNumber = meterDetailsItem.MeterNumber,
                Model = GetModel(meterDetailsItem),
                Manufacturer = NonApplicable,
                Configuration = meterDetailsItem.Description,
                MeterType = ConvertMeterType(meterDetailsItem.MeterType),
            };
        }

        private static string GetModel(MeterDetailsItem meterDetailsItem)
        {
            return string.IsNullOrEmpty(meterDetailsItem.ImpellerNumber)
                ? NonApplicable
                : meterDetailsItem.ImpellerNumber;
        }

        private MeterType ConvertMeterType(Dtos.MeterType? meterType)
        {
            switch (meterType)
            {
                case Dtos.MeterType.AcousticDopplerCurrentProfiler:
                    return MeterType.Adcp;
                case Dtos.MeterType.ElectroMagneticCurrentMeter:
                    return MeterType.ElectromagneticVelocityMeter;
                case Dtos.MeterType.RotatingElementCurrentMeter:
                    return MeterType.PriceAa;
                default:
                    return MeterType.Unspecified;
            }
        }

        private IEnumerable<MeterCalibrationEquation> CreateCalibrationEquations(
            MeterDetailsItem meterDetailsItem)
        {
            var orderedCalibrations = meterDetailsItem.Calibrations.OrderBy(c => c.MinRotationSpeed).ToList();
            for (var i = 0; i < meterDetailsItem.Calibrations.Count; i++)
            {
                var calibrationEquation = CreateCalibrationEquation(orderedCalibrations, i);

                yield return calibrationEquation;
            }
        }

        private MeterCalibrationEquation CreateCalibrationEquation(
            IReadOnlyList<MeterCalibrationItem> pocketGaugerCalibrations, int i)
        {
            return new MeterCalibrationEquation
            {
                RangeStart = pocketGaugerCalibrations[i].MinRotationSpeed,
                RangeEnd = GetRangeEnd(pocketGaugerCalibrations, i),
                Slope = pocketGaugerCalibrations[i].Factor,
                Intercept = pocketGaugerCalibrations[i].Constant,
                InterceptUnit = _parseContext.GetParameterDefaultUnit(ParametersAndMethodsConstants.VelocityParameterId)
            };
        }

        private static double? GetRangeEnd(IReadOnlyList<MeterCalibrationItem> pocketGaugerCalibrations, int i)
        {
            var isLastCalibration = i == pocketGaugerCalibrations.Count - 1;

            const int lastCalibrationRangeSize = 100;
            return isLastCalibration
                ? pocketGaugerCalibrations[i].MinRotationSpeed + lastCalibrationRangeSize
                : pocketGaugerCalibrations[i + 1].MinRotationSpeed;
        }
    }
}
