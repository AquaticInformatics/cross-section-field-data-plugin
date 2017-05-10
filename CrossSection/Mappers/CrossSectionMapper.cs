using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPlugInCore.Units;
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
            var unitSystem = CreateUnitSystemWithRequiredUnits(crossSectionSurvey);

            var crossSectionSurveyFactory = new CrossSectionSurveyFactory(unitSystem)
            {
                DefaultChannelName = crossSectionSurvey.GetFieldValueWithDefault(Channel, CrossSectionParserConstants.DefaultChannelName),
                DefaultRelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                DefaultStartPointType = crossSectionSurvey.GetFieldValue(StartBank).ToStartPointType()
            };

            var startTime = crossSectionSurvey.GetFieldValue(StartDate).ToDateTimeOffset();
            var endTime = crossSectionSurvey.GetFieldValue(EndDate).ToDateTimeOffset();
            var surveyPeriod = new DateTimeInterval(startTime, endTime);

            var newCrossSectionSurvey = crossSectionSurveyFactory.CreateCrossSectionSurvey(surveyPeriod);

            var stageValue = crossSectionSurvey.GetFieldValue(Stage).ToDouble();
            newCrossSectionSurvey.StageMeasurement = stageValue == null ? null : new Measurement(stageValue.Value, unitSystem.DistanceUnitId);
            newCrossSectionSurvey.Party = crossSectionSurvey.GetFieldValue(Party);
            newCrossSectionSurvey.Comments = crossSectionSurvey.GetFieldValue(Comment);

            newCrossSectionSurvey.Points(_crossSectionPointMapper.MapPoints(crossSectionSurvey.Points));

            return newCrossSectionSurvey;
        }

        private static UnitSystem CreateUnitSystemWithRequiredUnits(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var distanceUnit = crossSectionSurvey.GetFieldValue(Unit);
            return new UnitSystem {DistanceUnitId = distanceUnit};
        }
    }
}
