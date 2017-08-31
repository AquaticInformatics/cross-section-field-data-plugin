using System.Collections.Generic;
using System.IO;

namespace Server.Plugins.FieldVisit.StageDischarge.Interfaces
{
    public interface IDataParser<TRecord> where TRecord : class, ISelfValidator
    {
        double InvalidRecords { get; }
        double ValidRecords { get; }
        double SkippedRecords { get; }
        IEnumerable<TRecord> ParseInputData(Stream inputStream);
    }
}
