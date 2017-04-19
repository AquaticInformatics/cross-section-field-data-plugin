using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using CrossSectionSurvey = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection.CrossSectionSurvey;
using static Server.Plugins.FieldVisit.CrossSection.Helpers.CrossSectionDataFields;

namespace Server.Plugins.FieldVisit.CrossSection.Mappers
{
    public class CrossSectionMapper : ICrossSectionMapper
    {
        private readonly ICrossSectionPointMapper _crossSectionPointMapper;

        public CrossSectionMapper(ICrossSectionPointMapper crossSectionPointMapper)
        {
            _crossSectionPointMapper = crossSectionPointMapper;
        }

        public CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var commonUnit = crossSectionSurvey.GetFieldValue(Unit);

            var crossSectionSurveyFactory = new CrossSectionSurveyFactory()
            {
                DefaultChannelName = crossSectionSurvey.GetFieldValueWithDefault(Channel, CrossSectionParserConstants.DefaultChannelName),
                DefaultRelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                DefaultStartPointType = crossSectionSurvey.GetFieldValue(StartBank).ToStartPointType(),
                DefaultDistanceUnitId = commonUnit,
                DefaultDepthUnitId = commonUnit
            };

            var startTime = crossSectionSurvey.GetFieldValue(StartDate).ToDateTimeOffset();
            var endTime = crossSectionSurvey.GetFieldValue(EndDate).ToDateTimeOffset();
            var surveyPeriod = new DateTimeInterval(startTime, endTime);

            var party = crossSectionSurvey.GetFieldValue(Party);

            var newCrossSectionSurvey = crossSectionSurveyFactory.CreateCrossSectionSurvey(surveyPeriod, party);

            var stageValue = crossSectionSurvey.GetFieldValue(Stage).ToDouble();
            newCrossSectionSurvey.StageMeasurement = stageValue == null ? null : new Measurement(stageValue.Value, commonUnit);
            newCrossSectionSurvey.Comments = crossSectionSurvey.GetFieldValue(Comment);

            newCrossSectionSurvey.Points(_crossSectionPointMapper.MapPoints(crossSectionSurvey.Points));

            return newCrossSectionSurvey;
        }
    }
}
