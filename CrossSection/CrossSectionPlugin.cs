using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using Server.Plugins.FieldVisit.CrossSection.Mappers;
using Server.Plugins.FieldVisit.CrossSection.Model;
using Server.Plugins.FieldVisit.CrossSection.Parsers;

namespace Server.Plugins.FieldVisit.CrossSection
{
    public class CrossSectionPlugin : IFieldDataPlugIn
    {
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            var crossSectionSurvey = ProcessFileStream(CreateCrossSectionParser(), fileStream);

            var parsedResult = CreateParsedResult(CreateParsedResultMapper(context), crossSectionSurvey);

            return new[] { parsedResult };
        }

        private static ICrossSectionParser CreateCrossSectionParser()
        {
            return new CrossSectionSurveyParser();
        }

        private static ParsedResult CreateParsedResult(IParsedResultMapper mapper, CrossSectionSurvey crossSectionSurvey)
        {
            return mapper.CreateParsedResult(crossSectionSurvey);
        }

        private static IParsedResultMapper CreateParsedResultMapper(IParseContext context)
        {
            return new ParsedResultMapper(context, new CrossSectionMapper(context, new CrossSectionPointMapper()));
        }

        private static CrossSectionSurvey ProcessFileStream(ICrossSectionParser parser, Stream fileStream)
        {
            return parser.ParseFile(fileStream);
        }
    }
}
