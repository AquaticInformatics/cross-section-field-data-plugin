using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IPanelParser
    {
        IReadOnlyCollection<PanelItem> Parse(PocketGaugerFiles pocketGaugerFiles);
    }
}
