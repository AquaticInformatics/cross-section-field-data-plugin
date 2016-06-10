using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class VerticalItem
    {
        [XmlIgnore]
        public double? VerticalId { get; set; }

        [XmlElement("VERTICAL_ID")]
        public string VerticalIdProxy
        {
            get { return DoubleHelper.Serialize(VerticalId); }
            set { VerticalId = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? VerticalNo { get; set; }

        [XmlElement("VERTICAL_NO")]
        public string VerticalNoProxy
        {
            get { return DoubleHelper.Serialize(VerticalNo); }
            set { VerticalNo = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public int? MeasurementNo { get; set; }

        [XmlElement("MEASUREMENT_NO")]
        public string MeasurementNoProxy
        {
            get { return IntHelper.Serialize(MeasurementNo); }
            set { MeasurementNo = IntHelper.Parse(value); }
        }

        [XmlElement("PANEL_ID")]
        public string PanelId { get; set; }

        [XmlIgnore]
        public double? Distance { get; set; }

        [XmlElement("DISTANCE")]
        public string DistanceProxy
        {
            get { return DoubleHelper.Serialize(Distance); }
            set { Distance = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Depth { get; set; }

        [XmlElement("DEPTH")]
        public string DepthProxy
        {
            get { return DoubleHelper.Serialize(Depth); }
            set { Depth = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleFromSurface { get; set; }

        [XmlElement("SAMPLE_FROM_SURFACE")]
        public string SampleFromSurfaceProxy
        {
            get { return BooleanHelper.Serialize(SampleFromSurface); }
            set { SampleFromSurface = BooleanHelper.Parse(value); }
        }

        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlIgnore]
        public double? SamplePosition { get; set; }

        [XmlElement("SAMPLE_POSITION")]
        public string SamplePositionProxy
        {
            get { return DoubleHelper.Serialize(SamplePosition); }
            set { SamplePosition = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? ExposureTime { get; set; }

        [XmlElement("EXPOSURE_TIME")]
        public string ExposureTimeProxy
        {
            get { return DoubleHelper.Serialize(ExposureTime); }
            set { ExposureTime = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Revs { get; set; }

        [XmlElement("REVS")]
        public string RevsProxy
        {
            get { return DoubleHelper.Serialize(Revs); }
            set { Revs = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Velocity { get; set; }

        [XmlElement("VELOCITY")]
        public string VelocityProxy
        {
            get { return DoubleHelper.Serialize(Velocity); }
            set { Velocity = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Stage { get; set; }

        [XmlElement("STAGE")]
        public string StageProxy
        {
            get { return DoubleHelper.Serialize(Stage); }
            set { Stage = DoubleHelper.Parse(value); }
        }

        [XmlElement("SITE_ID")]
        public string SiteId { get; set; }

        [XmlElement("GAUGING_ID")]
        public string GaugingId { get; set; }
    }
}
