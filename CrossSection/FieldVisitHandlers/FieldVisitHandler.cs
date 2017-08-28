using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;

namespace Server.Plugins.FieldVisit.CrossSection.FieldVisitHandlers
{
    public class FieldVisitHandler : IFieldVisitHandler
    {
        private readonly IFieldDataResultsAppender _fieldDataResultsAppender;

        public FieldVisitHandler(IFieldDataResultsAppender fieldDataResultsAppender)
        {
            _fieldDataResultsAppender = fieldDataResultsAppender;
        }

        public FieldVisitInfo GetFieldVisit(string locationIdentifier, CrossSectionSurvey crossSectionSurvey)
        {
            var location = _fieldDataResultsAppender.GetLocationByIdentifier(locationIdentifier);
            return CreateFieldVisit(location, crossSectionSurvey);
        }

        protected FieldVisitInfo CreateFieldVisit(LocationInfo location, CrossSectionSurvey crossSectionSurvey)
        {
            var fieldVisit = new FieldVisitDetails(crossSectionSurvey.SurveyPeriod) {Party = crossSectionSurvey.Party};
            return _fieldDataResultsAppender.AddFieldVisit(location, fieldVisit);
        }
    }
}
