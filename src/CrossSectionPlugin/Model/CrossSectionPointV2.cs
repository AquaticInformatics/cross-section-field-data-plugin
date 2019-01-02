using System;
using CrossSectionPlugin.Helpers;
using FileHelpers;

namespace CrossSectionPlugin.Model
{
    [DelimitedRecord(CrossSectionParserConstants.CrossSectionPointDataSeparator)]
    public class CrossSectionPointV2 : ICrossSectionPoint
    {
        [FieldOrder(1), FieldTrim(TrimMode.Both), FieldOptional]
        public int? PointOrder;

        [FieldOrder(2), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Distance;

        [FieldOrder(3), FieldTrim(TrimMode.Both), FieldOptional]
        public double? Elevation;

        [FieldOrder(4), FieldTrim(TrimMode.Both), FieldQuoted, FieldOptional]
        public string Comment;

        public bool IsEmptyPoint()
        {
            return !PointOrder.HasValue && !Distance.HasValue && !Elevation.HasValue && string.IsNullOrWhiteSpace(Comment);
        }

        public override string ToString()
        {
            return FormattableString.Invariant($"PointOrder='{PointOrder}' Distance='{Distance}' Elevation='{Elevation}' Comment='{Comment}'");
        }
    }
}
