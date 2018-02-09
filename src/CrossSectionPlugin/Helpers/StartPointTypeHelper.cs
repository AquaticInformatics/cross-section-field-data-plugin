using System;
using System.Linq;
using CrossSectionPlugin.Exceptions;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;

namespace CrossSectionPlugin.Helpers
{
    public class StartPointTypeHelper
    {
        private static readonly string[] ValidLeftPrefixes = {"Left", "LEW"};
        private static readonly string[] ValidRightPrefixes = {"Right", "REW"};

        public static StartPointType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw CreateInvalidStartBankException();

            var startPoint = value.Replace(" ", string.Empty);

            StartPointType startPointEnum;

            if (Enum.TryParse(startPoint, true, out startPointEnum))
            {
                if (startPointEnum == StartPointType.LeftEdgeOfWater || startPointEnum == StartPointType.RightEdgeOfWater)
                    return startPointEnum;

                throw CreateInvalidStartBankException();
            }

            return AttemptToInferStartPoint(startPoint);
        }

        private static InvalidStartBankException CreateInvalidStartBankException()
        {
            return new InvalidStartBankException("StartBank must be set to 'LeftEdgeOfWater' or 'RightEdgeOfWater'");
        }

        private static StartPointType AttemptToInferStartPoint(string startPoint)
        {
            if (StartsWithAnyPrefix(startPoint, ValidLeftPrefixes))
                return StartPointType.LeftEdgeOfWater;

            if(StartsWithAnyPrefix(startPoint, ValidRightPrefixes))
                return StartPointType.RightEdgeOfWater;

            throw CreateInvalidStartBankException();
        }

        private static bool StartsWithAnyPrefix(string startPoint, string[] validPrefixes)
        {
            return validPrefixes.Any(prefix => startPoint.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
