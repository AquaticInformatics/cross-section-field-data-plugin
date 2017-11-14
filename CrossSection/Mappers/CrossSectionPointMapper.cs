using System;
using System.Collections.Generic;
using System.Linq;
using FieldDataPluginFramework.DataModel.CrossSection;
using Server.Plugins.FieldVisit.CrossSection.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Interfaces;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.CrossSection.Mappers
{
    public class CrossSectionPointMapper : ICrossSectionPointMapper
    {
        public ICollection<CrossSectionPoint> MapPoints(List<Model.CrossSectionPoint> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            return points.Where(point => point != null && !point.IsEmptyPoint()).Select(ToPoint).ToList();
        }

        private static CrossSectionPoint ToPoint(Model.CrossSectionPoint point)
        {
            if (point.Distance.HasValue & point.Elevation.HasValue)
            {
                return new CrossSectionPoint(point.Distance.Value, point.Elevation.Value)
                {
                    Comments = point.Comment
                };
            }
            throw new CrossSectionSurveyDataFormatException(Invariant($"The Cross-Section Point: '{point}' must have both a Distance and Elevation"));
        }
    }
}
