using System;
using System.Collections.Generic;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using static System.FormattableString;

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

        public string GetMetadataValue(string metadata)
        {
            if (string.IsNullOrWhiteSpace(metadata))
                throw new ArgumentNullException(nameof(metadata));

            if (Metadata.ContainsKey(metadata))
            {
                return Metadata[metadata];
            }

            throw new ParsingFailedException(Invariant($"Metadata record '{metadata}' was not found."));
        }
    }
}
