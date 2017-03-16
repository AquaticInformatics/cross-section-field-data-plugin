using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.Plugins.FieldVisit.CrossSection.Helpers;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using CrossSectionSurvey = Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.CrossSection.CrossSectionSurvey;
using static Server.Plugins.FieldVisit.CrossSection.Helpers.CrossSectionDataFields;

namespace Server.Plugins.FieldVisit.CrossSection.Mappers
{
    public class CrossSectionMapper : ICrossSectionMapper
    {
        private readonly IParseContext _parseContext;
        private readonly ICrossSectionPointMapper _crossSectionPointMapper;

        public CrossSectionMapper(IParseContext parseContext, ICrossSectionPointMapper crossSectionPointMapper)
        {
            _parseContext = parseContext;
            _crossSectionPointMapper = crossSectionPointMapper;
        }

        public CrossSectionMapper(ICrossSectionPointMapper crossSectionPointMapper)
        {
            _parseContext = null;
            _crossSectionPointMapper = crossSectionPointMapper;
        }

        public CrossSectionSurvey MapCrossSection(ILocationInfo location, Model.CrossSectionSurvey crossSectionSurvey)
        {
            var commonStageUnit = FindUnit(crossSectionSurvey.GetFieldValue(Unit));

            return new CrossSectionSurvey
            {
                Party = crossSectionSurvey.GetFieldValue(Party),
                Comments = crossSectionSurvey.GetFieldValue(Comment),
                StartTime = crossSectionSurvey.GetFieldValue(StartDate).ToDateTimeOffset(),
                EndTime = crossSectionSurvey.GetFieldValue(EndDate).ToDateTimeOffset(),
                Stage = crossSectionSurvey.GetFieldValue(Stage).ToDouble(),
                StartPoint = crossSectionSurvey.GetFieldValue(StartBank).ToStartPointType(),
                RelativeLocation = FindRelativeLocation(location, crossSectionSurvey.GetFieldValue(RelativeLocation)),
                RelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                Channel = FindChannelInfo(location, crossSectionSurvey.GetFieldValue(Channel)),
                ChannelName = crossSectionSurvey.GetFieldValueWithDefault(Channel, CrossSectionParserConstants.DefaultChannelName),
                DepthUnit = commonStageUnit,
                DepthUnitId = commonStageUnit.UnitId,
                DistanceUnit = commonStageUnit,
                DistanceUnitId = commonStageUnit.UnitId,
                StageUnit = commonStageUnit,
                StageUnitId = commonStageUnit.UnitId,
                CrossSectionPoints = _crossSectionPointMapper.MapPoints(crossSectionSurvey.Points)
            };
        }

        public CrossSectionSurvey MapCrossSection(Model.CrossSectionSurvey crossSectionSurvey)
        {
            var commonUnit = crossSectionSurvey.GetFieldValue(Unit);

            return new CrossSectionSurvey
            {
                Party = crossSectionSurvey.GetFieldValue(Party),
                Comments = crossSectionSurvey.GetFieldValue(Comment),
                StartTime = crossSectionSurvey.GetFieldValue(StartDate).ToDateTimeOffset(),
                EndTime = crossSectionSurvey.GetFieldValue(EndDate).ToDateTimeOffset(),
                Stage = crossSectionSurvey.GetFieldValue(Stage).ToDouble(),
                StartPoint = crossSectionSurvey.GetFieldValue(StartBank).ToStartPointType(),
                RelativeLocationName = crossSectionSurvey.GetFieldValueWithDefault(RelativeLocation, CrossSectionParserConstants.DefaultRelativeLocationName),
                ChannelName = crossSectionSurvey.GetFieldValueWithDefault(Channel, CrossSectionParserConstants.DefaultChannelName),
                DepthUnitId = commonUnit,
                DistanceUnitId = commonUnit,
                StageUnitId = commonUnit,
                CrossSectionPoints = _crossSectionPointMapper.MapPoints(crossSectionSurvey.Points)
            };
        }

        private IUnit FindUnit(string unit)
        {
            return _parseContext.LengthUnits.FirstOrDefault(u => u.Name.EqualsOrdinalIgnoreCase(unit)) ??
                _parseContext.GageHeightParameter.DefaultUnit;
        }

        private static IRelativeLocationInfo FindRelativeLocation(ILocationInfo location, string relativeLocationName)
        {
            if (string.IsNullOrWhiteSpace(relativeLocationName))
                return GetOrCreateRelativeLocation(location, CrossSectionParserConstants.DefaultRelativeLocationName);

            return GetOrCreateRelativeLocation(location, relativeLocationName);
        }

        private static IRelativeLocationInfo GetOrCreateRelativeLocation(ILocationInfo location, string relativeLocationName)
        {
            return location.RelativeLocations.FirstOrDefault(l => l.RelativeLocationName.EqualsOrdinalIgnoreCase(relativeLocationName)) ??
                   location.CreateNewRelativeLocation(relativeLocationName);
        }

        private static IChannelInfo FindChannelInfo(ILocationInfo location, string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName))
                return GetOrCreateChannel(location, CrossSectionParserConstants.DefaultChannelName);

            return GetOrCreateChannel(location, channelName);
        }

        private static IChannelInfo GetOrCreateChannel(ILocationInfo location, string channelName)
        {
            return location.Channels.FirstOrDefault(c => c.ChannelName.EqualsOrdinalIgnoreCase(channelName)) ??
                   location.CreateNewChannel(channelName);
        }
    }
}
