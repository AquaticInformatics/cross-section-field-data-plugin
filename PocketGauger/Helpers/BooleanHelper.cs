namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class BooleanHelper
    {
        private const string True = "True";
        private const string False = "False";

        public static bool Parse(string value)
        {
            return value == True;
        }

        public static string Serialize(bool value)
        {
            return value ? True : False;
        }
    }
}
