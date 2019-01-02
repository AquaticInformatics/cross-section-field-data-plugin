using System;
using System.Collections.Generic;
using System.Linq;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Interfaces;
using CrossSectionPlugin.Model;
using FieldDataPluginFramework.DataModel.CrossSection;
using static System.FormattableString;

namespace CrossSectionPlugin.Mappers
{
    public class CrossSectionPointMapper : ICrossSectionPointMapper
    {
        public List<CrossSectionPoint> MapPoints(List<ICrossSectionPoint> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            var filteredPoints = points.Where(point => point != null && !point.IsEmptyPoint()).ToList();

            if (!filteredPoints.Any())
                return new List<CrossSectionPoint>();

            var crossSectionPoint = filteredPoints.First();
            switch (crossSectionPoint)
            {
                case CrossSectionPointV1 _:
                    {
                        return filteredPoints.Cast<CrossSectionPointV1>()
                            .OrderBy(point => point.Distance)
                            .Select(ToPoint)
                            .ToList();
                    }
                case CrossSectionPointV2 _:
                    {
                        return filteredPoints.Cast<CrossSectionPointV2>()
                            .OrderBy(point => point.PointOrder)
                            .Select(ToPoint)
                            .ToList();
                    }
                default:
                    {
                        throw new CrossSectionCsvFormatException(Invariant($"Unsupported Cross-Section type '{crossSectionPoint.GetType()}'"));
                    }
            }
        }

        private static CrossSectionPoint ToPoint(CrossSectionPointV1 crossSectionPoint, int index)
        {
            if (crossSectionPoint.Distance.HasValue && crossSectionPoint.Elevation.HasValue)
            {
                return CreateCrossSectionPoint(index + 1, crossSectionPoint.Distance.Value,
                    crossSectionPoint.Elevation.Value, crossSectionPoint.Comment);
            }
            throw new CrossSectionSurveyDataFormatException(Invariant(
                $"The Cross-Section Point: '{crossSectionPoint}' must have both a {nameof(CrossSectionPointV1.Distance)} and {nameof(CrossSectionPointV1.Elevation)}"));
        }

        private static CrossSectionPoint CreateCrossSectionPoint(int pointOrder, double distance, double elevation, string comment)
        {
            return new CrossSectionPoint(pointOrder, distance, elevation)
            {
                Comments = comment
            };
        }

        private static CrossSectionPoint ToPoint(CrossSectionPointV2 crossSectionPoint)
        {
            if (crossSectionPoint.PointOrder.HasValue && crossSectionPoint.Distance.HasValue && crossSectionPoint.Elevation.HasValue)
            {
                return CreateCrossSectionPoint(crossSectionPoint.PointOrder.Value, crossSectionPoint.Distance.Value,
                    crossSectionPoint.Elevation.Value, crossSectionPoint.Comment);
            }
            throw new CrossSectionSurveyDataFormatException(Invariant(
                $"The Cross-Section Point: '{crossSectionPoint}' must have {nameof(CrossSectionPointV2.PointOrder)}, {nameof(CrossSectionPointV2.Distance)} and {nameof(CrossSectionPointV2.Elevation)}"));
        }
    }
}
