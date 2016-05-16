using System.Collections.Generic;
using System.IO;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser
    {
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            throw new System.NotImplementedException();
        }
    }
}
