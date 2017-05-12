using System;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.ChannelMeasurements;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
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
                if (startPointEnum == StartPointType.Unspecified) throw CreateInvalidStartBankException();

                return startPointEnum;
            }

            return AttemptToInferStartPoint(startPoint);
        }

        private static ParsingFailedException CreateInvalidStartBankException()
        {
            return new ParsingFailedException("StartBank must be set to 'LeftEdgeOfWater' or 'RightEdgeOfWater'");
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
