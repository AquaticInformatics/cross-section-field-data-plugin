using System;
using FieldDataPluginFramework.DataModel;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class MeasurementHelper
    {
        public static Measurement AsDischargeMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsHelper.UnitSystem.DischargeUnitId);
        }

        public static Measurement AsDistanceMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsHelper.UnitSystem.DistanceUnitId);
        }

        public static Measurement AsVelocityMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsHelper.UnitSystem.VelocityUnitId);
        }

        private static Measurement CreateMeasurementOrThrowIfNull(double? value, string unit)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Measurement(value.Value, unit);
        }
    }
}
