using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection;
using Server.BusinessInterfaces.FieldDataPluginCore.Units;
﻿using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using CrossSectionSurvey = Server.BusinessInterfaces.FieldDataPluginCore.DataModel.CrossSection.CrossSectionSurvey;
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

            var stageMeasurement = CreateStageMeasurement(crossSectionSurvey, unitSystem);
            newCrossSectionSurvey.StageMeasurement = stageMeasurement;

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

        private static Measurement CreateStageMeasurement(Model.CrossSectionSurvey crossSectionSurvey, UnitSystem unitSystem)
        {
            var stageValue = crossSectionSurvey.GetFieldValue(Stage).ToNullableDouble();
            if (stageValue == null)
                throw new ValidationException("Stage value is required");

            return new Measurement(stageValue.Value, unitSystem.DistanceUnitId);
        }
    }
}
