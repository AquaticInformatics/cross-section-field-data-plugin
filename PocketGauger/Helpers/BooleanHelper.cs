using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class BooleanHelper
    {
        public const string True = "True";
        public const string False = "False";

        public static bool Parse(string value)
        {
            if (value == True) return true;
            if (value == False) return false;

            throw new ParsingFailedException(string.Format("{0} is not a valid boolean value", value));
        }

        public static string Serialize(bool value)
        {
            return value ? True : False;
        }
    }
}
