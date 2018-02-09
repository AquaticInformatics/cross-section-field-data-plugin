using System;
using System.Collections.Generic;
using CrossSectionPlugin.Helpers;
using CrossSectionPlugin.Interfaces;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.CrossSection;
using FieldDataPluginFramework.Units;
using CrossSectionSurvey = FieldDataPluginFramework.DataModel.CrossSection.CrossSectionSurvey;

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
                DefaultChannelName = crossSectionSurvey.GetFieldValueWithDefault(CrossSectionDataFields.Channel, CrossSectionParserConstants.DefaultChannelName),
                DefaultRelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(CrossSectionDataFields.RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                DefaultStartPointType = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.StartBank).ToStartPointType()
            };

            var startTime = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.StartDate).ToDateTimeOffset();
            var endTime = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.EndDate).ToDateTimeOffset();
            var surveyPeriod = new DateTimeInterval(startTime, endTime);


            var newCrossSectionSurvey = crossSectionSurveyFactory.CreateCrossSectionSurvey(surveyPeriod);

            newCrossSectionSurvey.StageMeasurement = CreateStageMeasurement(crossSectionSurvey, unitSystem);

            newCrossSectionSurvey.Party = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Party);
            newCrossSectionSurvey.Comments = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Comment);

            var mappedPoints = (List<CrossSectionPoint>)_crossSectionPointMapper.MapPoints(crossSectionSurvey.Points);
            newCrossSectionSurvey.CrossSectionPoints = mappedPoints;
                

            return newCrossSectionSurvey;
        }

        private static UnitSystem CreateUnitSystemWithRequiredUnits(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var distanceUnit = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Unit);
            return new UnitSystem {DistanceUnitId = distanceUnit};
        }

        private static Measurement CreateStageMeasurement(Model.CrossSectionSurvey crossSectionSurvey, UnitSystem unitSystem)
        {
            var stageValue = crossSectionSurvey.GetFieldValue(CrossSectionDataFields.Stage).ToNullableDouble();
            if (stageValue == null)
                throw new ArgumentNullException("Stage value is required");

            return new Measurement(stageValue.Value, unitSystem.DistanceUnitId);
        }
    }
}
