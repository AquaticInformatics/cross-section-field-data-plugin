using System;
using System.Collections.Generic;
using Server.Plugins.FieldVisit.CrossSection.Helpers;

namespace Server.Plugins.FieldVisit.CrossSection.Model
{
    public class CrossSectionSurvey
    {
        public CrossSectionSurvey()
        {
            CsvFileVersion = new Version(CrossSectionParserConstants.DefaultVersion);
            Metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Points = new List<CrossSectionPoint>();
        }

        public Version CsvFileVersion { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        public List<CrossSectionPoint> Points { get; set; }
    }
}
