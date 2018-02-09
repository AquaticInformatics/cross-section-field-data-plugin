using System.IO;
using CrossSectionPlugin.FieldVisitHandlers;
using FieldDataPluginFramework;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.Results;

namespace CrossSectionPlugin
{
    public class CrossSectionPlugin : IFieldDataPlugin
    {
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            var fieldVisitHandler = new FieldVisitHandler(fieldDataResultsAppender);

            var fileParser = new CrossSectionFileParser(fieldDataResultsAppender, fieldVisitHandler, logger);

            return fileParser.ParseFile(fileStream);
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender, ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }
    }
}
