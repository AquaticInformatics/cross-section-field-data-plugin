using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public static class CrossSectionParserConstants
    {
        public const string Header = "AQUARIUS Cross-Section CSV";
        public const string DefaultVersion = "1.0";

        public const string MetadataSeparator = ":";
        public const string DataRecordSeparator = ",";

        public const string DefaultChannelName = "Main";
        public const string DefaultRelativeLocationName = "At the control";
        public const StartPointType DefaultStartPointType = StartPointType.Unspecified;
    }
}
