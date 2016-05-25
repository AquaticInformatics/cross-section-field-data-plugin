using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public static class GaugingSummaryParser
    {
        public static GaugingSummary Parse(PocketGaugerFiles pocketGaugerFiles)
        {
            return pocketGaugerFiles.ParseType<GaugingSummary>();
        }
    }
}
