using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.DataModel.Meters;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using MeterCalibration = FieldDataPluginFramework.DataModel.Meters.MeterCalibration;
using MeterType = FieldDataPluginFramework.DataModel.Meters.MeterType;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class MeterCalibrationMapper : IMeterCalibrationMapper
    {
        public static string NonApplicable = "N/A";

        public MeterCalibration Map(MeterDetailsItem meterDetailsItem)
        {
            var calibration = CreateCalibration(meterDetailsItem);

            foreach (var equation in CreateCalibrationEquations(meterDetailsItem))
            {
                calibration.Equations.Add(equation);
            }

            return calibration;
        }

        private static MeterCalibration CreateCalibration(MeterDetailsItem meterDetailsItem)
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

        private static MeterType ConvertMeterType(Dtos.MeterType? meterType)
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
            var orderedCalibrations = GetOrderedCalibrations(meterDetailsItem);
            for (var i = 0; i < orderedCalibrations.Count; i++)
            {
                var calibrationEquation = CreateCalibrationEquation(orderedCalibrations, i);

                yield return calibrationEquation;
            }
        }

        private static List<MeterCalibrationItem> GetOrderedCalibrations(MeterDetailsItem meterDetailsItem)
        {
            return meterDetailsItem.Calibrations
                .Where(calibration => calibration.Constant.HasValue && calibration.Factor.HasValue)
                .OrderBy(c => c.MinRotationSpeed)
                .ToList();
        }

        private MeterCalibrationEquation CreateCalibrationEquation(
            IReadOnlyList<MeterCalibrationItem> pocketGaugerCalibrations, int i)
        {
            var calibration = pocketGaugerCalibrations[i];

            return new MeterCalibrationEquation
            {
                RangeStart = calibration.MinRotationSpeed,
                RangeEnd = GetRangeEnd(pocketGaugerCalibrations, i),
                Slope = calibration.Factor.GetValueOrDefault(),
                Intercept = calibration.Constant.GetValueOrDefault(),
                InterceptUnitId = ParametersAndMethodsHelper.UnitSystem.VelocityUnitId
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
