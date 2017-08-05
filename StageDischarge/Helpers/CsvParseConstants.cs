using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Plugins.FieldVisit.StageDischarge.Helpers
{
    public class CsvParserConstants
    {
        public const string FieldDelimiter = ",";
        public const string CommentMarker = "#";
        public const int MaximumErrorCount = 5;
        public const string DefaultChannelName = "Main";
        public const string DefaultPartyName = "Unknown";
    }
}
