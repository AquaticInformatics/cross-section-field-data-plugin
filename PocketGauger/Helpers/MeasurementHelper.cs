using System;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class MeasurementHelper
    {
        public static Measurement AsDischargeMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsConstants.DischargeUnitId);
        }

        public static Measurement AsDistanceMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsConstants.DistanceUnitId);
        }

        public static Measurement AsAreaMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsConstants.AreaUnitId);
        }

        public static Measurement AsVelocityMeasurement(this double? value)
        {
            return CreateMeasurementOrThrowIfNull(value, ParametersAndMethodsConstants.VelocityUnitId);
        }

        private static Measurement CreateMeasurementOrThrowIfNull(double? value, string unit)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return new Measurement(value.Value, unit);
        }
    }
}
