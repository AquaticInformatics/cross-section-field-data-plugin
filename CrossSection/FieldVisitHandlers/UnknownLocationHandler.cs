using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class UnknownLocationHandler : FieldVisitHandlerBase
    {
         public UnknownLocationHandler(IFieldDataResultsAppender fieldDataResultsAppender)
            : base(fieldDataResultsAppender)
        {
        }

        public override IFieldVisit GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            var location = FieldDataResultsAppender.GetLocationByIdentifier(locationIdentifier);
            return CreateFieldVisit(location, crossSectionSurvey);
        }
    }
}
