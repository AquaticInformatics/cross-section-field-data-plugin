using System.Globalization;

namespace Server.Plugins.FieldVisit.CrossSection.Helpers
{
    public class DoubleHelper
    {
        public static double? Parse(string value)
        {
            double result;

            if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out result))
                return result;

            return null;
        }
    }
}
