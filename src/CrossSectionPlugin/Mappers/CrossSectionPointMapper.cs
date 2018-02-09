using System;
using System.Collections.Generic;
using System.Linq;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Interfaces;
using FieldDataPluginFramework.DataModel.CrossSection;

namespace CrossSectionPlugin.Mappers
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
            if (point.Distance.HasValue && point.Elevation.HasValue)
            {
                return new CrossSectionPoint(point.Distance.Value, point.Elevation.Value)
                {
                    Comments = point.Comment
                };
            }
            throw new CrossSectionSurveyDataFormatException(FormattableString.Invariant($"The Cross-Section Point: '{point}' must have both a Distance and Elevation"));
        }
    }
}
