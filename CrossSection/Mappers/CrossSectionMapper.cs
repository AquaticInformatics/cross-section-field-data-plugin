using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel;
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

            var startTime = crossSectionSurvey.GetFieldValue(StartDate).ToDateTimeOffset();
            var endTime = crossSectionSurvey.GetFieldValue(EndDate).ToDateTimeOffset();
            var party = crossSectionSurvey.GetFieldValue(Party);

            var stageValue = crossSectionSurvey.GetFieldValue(Stage).ToDouble();
            var stageMeasurement = stageValue == null ? null : new Measurement(stageValue.Value, commonUnit);

            return new CrossSectionSurvey(new DateTimeInterval(startTime, endTime), party)
            {
                Comments = crossSectionSurvey.GetFieldValue(Comment),
                StageMeasurement = stageMeasurement,
                StartPoint = crossSectionSurvey.GetFieldValue(StartBank).ToStartPointType(),
                RelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                ChannelName = crossSectionSurvey.GetFieldValueWithDefault(Channel, CrossSectionParserConstants.DefaultChannelName),
                DepthUnitId = commonUnit,
                DistanceUnitId = commonUnit,
                CrossSectionPoints = _crossSectionPointMapper.MapPoints(crossSectionSurvey.Points)
            };
        }
    }
}
