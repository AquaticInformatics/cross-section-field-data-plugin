using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge.Interfaces
{
    public interface IDataParser<RECORD> where RECORD : class, ISelfValidator
    {
        double InvalidRecords { get; }
        double ValidRecords { get; }
        double SkippedRecords { get; }
        IEnumerable<RECORD> ParseInputData(Stream inputStream);
    }
}
