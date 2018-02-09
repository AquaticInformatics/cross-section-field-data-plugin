using System;
using System.Collections.Generic;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;

namespace CrossSectionPlugin.Model
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

            throw new CrossSectionCsvFormatException(FormattableString.Invariant($"Field '{field}' was not found in the file."));
        }

        public string GetFieldValueWithDefault(string field, string defaultValue)
        {
            var fieldValue = GetFieldValue(field);

            return string.IsNullOrWhiteSpace(fieldValue) ? defaultValue : fieldValue;
        }
    }
}
