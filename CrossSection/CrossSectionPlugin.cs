using System.IO;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;

namespace Server.Plugins.FieldVisit.CrossSection
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
