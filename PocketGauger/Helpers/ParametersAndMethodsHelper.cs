using Server.BusinessInterfaces.FieldDataPluginCore.Units;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class ParametersAndMethodsHelper
    {
        public const string VelocityParameterId = "WV";
        public const string AreaParameterId = "RiverSectionArea";
        public const string WidthParameterId = "RiverSectionWidth";
        public const string DistanceToGageParameterId = "Distance";

        // TODO: Set from config file to allow customization
        public const string AreaUnitId = "m^2";
        public const string DistanceUnitId = "m";
        public const string VelocityUnitId = "m/s";
        public const string DischargeUnitId = "m^3/s";

        public const string GageHeightMethodCode = "HGFLOAT";

        public const string DefaultChannelName = "Main";

        public static UnitSystem DischargeSectionUnitSystem =
            new UnitSystem
            {
                AreaUnitId = AreaUnitId,
                DischargeUnitId = DischargeUnitId,
                DistanceUnitId = DistanceUnitId,
                VelocityUnitId = VelocityUnitId
            };
    }
}
