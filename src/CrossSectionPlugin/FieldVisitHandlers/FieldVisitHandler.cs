using CrossSectionPlugin.Interfaces;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.CrossSection;
using FieldDataPluginFramework.Results;

namespace CrossSectionPlugin.FieldVisitHandlers
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

        private FieldVisitInfo CreateFieldVisit(LocationInfo location, CrossSectionSurvey crossSectionSurvey)
        {
            var fieldVisit = new FieldVisitDetails(crossSectionSurvey.SurveyPeriod) {Party = crossSectionSurvey.Party};
            return _fieldDataResultsAppender.AddFieldVisit(location, fieldVisit);
        }
    }
}
