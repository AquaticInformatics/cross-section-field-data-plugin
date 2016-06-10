﻿using System.Globalization;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class DoubleHelper
    {
        public static double? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value == "-999999999.999")
                return default(double?);

            double result;

            return double.TryParse(value, out result) ? result : default(double?);
        }

        public static string Serialize(double? value)
        {
            return value?.ToString(NumberFormatInfo.InvariantInfo);
        }
    }
}
