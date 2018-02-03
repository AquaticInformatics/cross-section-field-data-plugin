using System.Collections.Generic;
using System.IO;

namespace Server.Plugins.FieldVisit.StageDischarge.Interfaces
{
    public interface IDataParser<TRecord> where TRecord : class, ISelfValidator
    {
        int ValidRecords { get; }
        int SkippedRecords { get; }
        string[] Errors { get; }

        IEnumerable<TRecord> ParseInputData(Stream inputStream);
    }
}
