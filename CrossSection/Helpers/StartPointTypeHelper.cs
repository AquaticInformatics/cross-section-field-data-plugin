using System;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public class StartPointTypeHelper
    {
        private const StartPointType DefaultStartPointType = CrossSectionParserConstants.DefaultStartPointType;
        private const string LeftPrefix = "Left";
        private const string RightPrefix = "Right";

        public static StartPointType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DefaultStartPointType;

            var startPoint = value.Replace(" ", string.Empty);

            StartPointType startPointEnum;

            if (Enum.TryParse(startPoint, true, out startPointEnum))
                return startPointEnum;

            return AttemptToInferStartPoint(startPoint);
        }

        private static StartPointType AttemptToInferStartPoint(string startPoint)
        {
            if (startPoint.StartsWith(LeftPrefix, StringComparison.OrdinalIgnoreCase))
                return StartPointType.LeftEdgeOfWater;

            if (startPoint.StartsWith(RightPrefix, StringComparison.OrdinalIgnoreCase))
                return StartPointType.RightEdgeOfWater;

            return DefaultStartPointType;
        }
    }
}
