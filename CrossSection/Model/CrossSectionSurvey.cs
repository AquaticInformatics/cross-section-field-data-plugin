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
            Fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Points = new List<CrossSectionPoint>();
        }

        public Version CsvFileVersion { get; set; }

        public IDictionary<string, string> Fields { get; set; }

        public List<CrossSectionPoint> Points { get; set; }

        public string GetFieldValue(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException(nameof(field));

            if (Fields.ContainsKey(field))
            {
                return Fields[field];
            }

            throw new ParsingFailedException(Invariant($"Field '{field}' was not found in the file."));
        }

        public string GetFieldValueWithDefault(string field, string defaultValue)
        {
            var fieldValue = GetFieldValue(field);

            return string.IsNullOrWhiteSpace(fieldValue) ? defaultValue : fieldValue;
        }
    }
}
