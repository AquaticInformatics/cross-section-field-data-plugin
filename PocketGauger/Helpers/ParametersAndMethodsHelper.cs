using FieldDataPluginFramework.Units;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class ParametersAndMethodsHelper
    {
        public const string VelocityParameterId = "WV";
        public const string AreaParameterId = "RiverSectionArea";
        public const string WidthParameterId = "RiverSectionWidth";
        public const string DistanceToGageParameterId = "Distance";

        // TODO: Set from config file to allow customization
        private const string AreaUnitId = "m^2";
        private const string DistanceUnitId = "m";
        private const string VelocityUnitId = "m/s";
        private const string DischargeUnitId = "m^3/s";

        public const string GageHeightMethodCode = "HGFLOAT";

        public const string DefaultChannelName = "Main";

        public static UnitSystem UnitSystem =
            new UnitSystem
            {
                AreaUnitId = AreaUnitId,
                DischargeUnitId = DischargeUnitId,
                DistanceUnitId = DistanceUnitId,
                VelocityUnitId = VelocityUnitId
            };
    }
}
