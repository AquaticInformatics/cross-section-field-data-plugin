using System;
using CrossSectionPlugin.Helpers;
using CrossSectionPlugin.Interfaces;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.CrossSection;
using FieldDataPluginFramework.Units;
using CrossSectionSurvey = FieldDataPluginFramework.DataModel.CrossSection.CrossSectionSurvey;
using static CrossSectionPlugin.Helpers.CrossSectionDataFields;

namespace CrossSectionPlugin.Mappers
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

            newCrossSectionSurvey.StageMeasurement = CreateStageMeasurement(crossSectionSurvey, unitSystem);

            newCrossSectionSurvey.Party = crossSectionSurvey.GetFieldValue(Party);
            newCrossSectionSurvey.Comments = crossSectionSurvey.GetFieldValue(Comment);

            newCrossSectionSurvey.CrossSectionPoints = _crossSectionPointMapper.MapPoints(crossSectionSurvey.Points);

            return newCrossSectionSurvey;
        }

        private static UnitSystem CreateUnitSystemWithRequiredUnits(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var distanceUnit = crossSectionSurvey.GetFieldValue(Unit);
            return new UnitSystem {DistanceUnitId = distanceUnit};
        }

        private static Measurement CreateStageMeasurement(Model.CrossSectionSurvey crossSectionSurvey, UnitSystem unitSystem)
        {
            var stageValue = crossSectionSurvey.GetFieldValue(Stage).ToNullableDouble();
            if (stageValue == null)
                throw new ArgumentNullException(nameof(Stage), "Stage value is required");

            return new Measurement(stageValue.Value, unitSystem.DistanceUnitId);
        }
    }
}
