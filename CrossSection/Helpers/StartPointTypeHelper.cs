using System;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeSubActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public class StartPointTypeHelper
    {
        private static readonly string[] ValidLeftPrefixes = {"Left", "LEW"};
        private static readonly string[] ValidRightPrefixes = {"Right", "REW"};

        public static StartPointType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ParsingFailedException("StartPoint must be specified");

            var startPoint = value.Replace(" ", string.Empty);

            StartPointType startPointEnum;

            if (Enum.TryParse(startPoint, true, out startPointEnum))
                return startPointEnum;

            return AttemptToInferStartPoint(startPoint);
        }


        private static StartPointType AttemptToInferStartPoint(string startPoint)
        {
            if (StartsWithAnyPrefix(startPoint, ValidLeftPrefixes))
                return StartPointType.LeftEdgeOfWater;

            if(StartsWithAnyPrefix(startPoint, ValidRightPrefixes))
                return StartPointType.RightEdgeOfWater;

            throw new ParsingFailedException(Invariant($"Start point is not valid: {startPoint}"));
        }

        private static bool StartsWithAnyPrefix(string startPoint, string[] validPrefixes)
        {
            return validPrefixes.Any(leftPrefix => startPoint.StartsWith(leftPrefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
