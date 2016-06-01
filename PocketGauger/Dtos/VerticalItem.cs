using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class VerticalItem
    {
        [XmlElement("VERTICAL_ID")]
        public double VerticalId { get; set; }

        [XmlElement("VERTICAL_NO")]
        public double VerticalNo { get; set; }

        [XmlElement("MEASUREMENT_NO")]
        public int MeasurementNo { get; set; }

        [XmlElement("PANEL_ID")]
        public string PanelId { get; set; }

        [XmlElement("DISTANCE")]
        public double Distance { get; set; }

        [XmlElement("DEPTH")]
        public double Depth { get; set; }

        [XmlElement("SAMPLE_FROM_SURFACE")]
        public string SampleFromSurfaceProxy { get; set; }

        [XmlIgnore]
        public bool SampleFromSurface
        {
            get { return BooleanHelper.Parse(SampleFromSurfaceProxy); }
            set { SampleFromSurfaceProxy = BooleanHelper.Serialize(value); }
        }

        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlElement("SAMPLE_POSITION")]
        public double SamplePosition { get; set; }

        [XmlElement("EXPOSURE_TIME")]
        public double ExposureTime { get; set; }

        [XmlElement("REVS")]
        public double Revs { get; set; }

        [XmlElement("VELOCITY")]
        public double Velocity { get; set; }

        [XmlElement("STAGE")]
        public double Stage { get; set; }

        [XmlElement("SITE_ID")]
        public string SiteId { get; set; }

        [XmlElement("GAUGING_ID")]
        public string GaugingId { get; set; }
    }
}
