using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IGaugingSummaryParser
    {
        GaugingSummary Parse(PocketGaugerFiles pocketGaugerFiles);
    }
}
