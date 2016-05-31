using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using MeterCalibration = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterCalibration;
using MeterType = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.Meters.MeterType;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class MeterCalibrationMapper
    {
        private readonly IParseContext _parseContext;

        public MeterCalibrationMapper(IParseContext parseContext)
        {
            _parseContext = parseContext;
        }

        public IReadOnlyDictionary<string, MeterCalibration> Map(
            IReadOnlyDictionary<string, MeterDetailsItem> meterDetails)
        {
            var fieldDataMeters = new Dictionary<string, MeterCalibration>();
            foreach (var keyValuePair in meterDetails)
            {
                var calibration = CreateCalibration(keyValuePair.Value);
                calibration.Equations = CreateCalibrationEquations(keyValuePair.Value).ToList();

                fieldDataMeters.Add(keyValuePair.Key, calibration);
            }

            return fieldDataMeters;
        }

        private MeterCalibration CreateCalibration(MeterDetailsItem meterDetailsItem)
        {
            return new MeterCalibration
            {
                SerialNumber = meterDetailsItem.MeterNumber,
                Model = meterDetailsItem.ImpellerNumber,
                Configuration = meterDetailsItem.Description,
                MeterType = ConvertMeterType(meterDetailsItem.MeterType),
            };
        }

        public MeterType ConvertMeterType(Dtos.MeterType? meterType)
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

        private static IEnumerable<MeterCalibrationEquation> CreateCalibrationEquations(
            MeterDetailsItem meterDetailsItem)
        {
            var orderedCalibrations = meterDetailsItem.Calibrations.OrderBy(c => c.MinRotationSpeed).ToList();
            for (var i = 0; i < meterDetailsItem.Calibrations.Count; i++)
            {
                var calibrationEquation = CreateCalibrationEquation(orderedCalibrations, i);

                yield return calibrationEquation;
            }
        }

        private static MeterCalibrationEquation CreateCalibrationEquation(
            IReadOnlyList<MeterCalibrationItem> pocketGaugerCalibrations, int i)
        {
            return new MeterCalibrationEquation
            {
                RangeStart = pocketGaugerCalibrations[i].MinRotationSpeed,
                RangeEnd = GetRangeEnd(pocketGaugerCalibrations, i),
                Slope = pocketGaugerCalibrations[i].Factor,
                Intercept = pocketGaugerCalibrations[i].Constant,
                //todo: will update this to retrive default unit for velocity from parseContext after AQ-18922 is merged
                InterceptUnit = null
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
