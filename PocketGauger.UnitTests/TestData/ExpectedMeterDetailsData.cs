using System;
using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public class ExpectedMeterDetailsData
    {
        public static IReadOnlyDictionary<string, MeterDetailsItem> CreateExpectedThreeMeterDetails()
        {
            return new Dictionary<string, MeterDetailsItem>
            {
                { "12345±", CreateFirstMeterDetails() },
                { "1507±1178-1566", CreateSecondMeterDetails() },
                { "75-396±42", CreateThirdMeterDetails() }
            };
        }

        private static MeterDetailsItem CreateFirstMeterDetails()
        {
            return new MeterDetailsItem
            {
                MeterId = "12345±",
                MeterNumber = "12345",
                ImpellerNumber = String.Empty,
                Description = String.Empty,
                MeterType = null,
                MeterTypeProxy = null,
                Calibrations = CreateCalibrationsForFirstMeter()
            };
        }

        private static IReadOnlyList<MeterCalibrationItem> CreateCalibrationsForFirstMeter()
        {
            return new List<MeterCalibrationItem>
            {
                new MeterCalibrationItem
                {
                    MeterId = "12345±",
                    MinRotationSpeed = 0,
                    Factor = 1,
                    Constant = 0
                }
            };
        }

        private static MeterDetailsItem CreateSecondMeterDetails()
        {
            return new MeterDetailsItem
            {
                MeterId = "1507±1178-1566",
                MeterNumber = "1507",
                ImpellerNumber = "1178-1566",
                Description = "Valeport",
                MeterType = MeterType.ElectroMagneticCurrentMeter,
                MeterTypeProxy = "45",
                Calibrations = CreateCalibrationsForSecondMeter()
            };
        }

        private static IReadOnlyList<MeterCalibrationItem> CreateCalibrationsForSecondMeter()
        {
            return new List<MeterCalibrationItem>
            {
                new MeterCalibrationItem
                {
                    MeterId = "1507±1178-1566",
                    MinRotationSpeed = 0.15,
                    Factor = 0.104,
                    Constant = 0.034
                },
                new MeterCalibrationItem
                {
                    MeterId = "1507±1178-1566",
                    MinRotationSpeed = 1.66,
                    Factor = 0.11,
                    Constant = 0.025
                },
                new MeterCalibrationItem
                {
                    MeterId = "1507±1178-1566",
                    MinRotationSpeed = 5.25,
                    Factor = 0.109,
                    Constant = 0.028
                }
            };
        }

        private static MeterDetailsItem CreateThirdMeterDetails()
        {
            return new MeterDetailsItem
            {
                MeterId = "75-396±42",
                MeterNumber = "75-396",
                ImpellerNumber = "42",
                Description = String.Empty,
                MeterType = MeterType.RotatingElementCurrentMeter,
                MeterTypeProxy = "44",
                Calibrations = CreateCalibrationsForThirdMeter()
            };
        }

        private static IReadOnlyList<MeterCalibrationItem> CreateCalibrationsForThirdMeter()
        {
            return new List<MeterCalibrationItem>
            {
                new MeterCalibrationItem
                {
                    MeterId = "75-396±42",
                    MinRotationSpeed = 1,
                    Factor = 3,
                    Constant = 2
                },
                new MeterCalibrationItem
                {
                    MeterId = "75-396±42",
                    MinRotationSpeed = 5,
                    Factor = 7,
                    Constant = 6
                }
            };
        }
    }
}
