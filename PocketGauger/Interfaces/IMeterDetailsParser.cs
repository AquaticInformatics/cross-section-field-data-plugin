using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IMeterDetailsParser
    {
        IReadOnlyDictionary<string, MeterDetailsItem> Parse(PocketGaugerFiles pocketGaugerFiles);
    }
}
