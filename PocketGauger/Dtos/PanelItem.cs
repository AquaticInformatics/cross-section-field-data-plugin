using System.Collections.Generic;
using System.Xml.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class PanelItem
    {
        [XmlElement("PANEL_ID")]
        public string PanelId { get; set; }

        [XmlElement("STAGE")]
        public double Stage { get; set; }

        [XmlElement("AREA")]
        public double Area { get; set; }

        [XmlElement("MEAN_VELOCITY")]
        public double MeanVelocity { get; set; }

        [XmlElement("FLOW")]
        public double Flow { get; set; }

        [XmlElement("VERTICAL_NUMBER")]
        public int VerticalNumber { get; set; }

        [XmlElement("DISTANCE")]
        public double Distance { get; set; }

        [XmlElement("DEPTH")]
        public double Depth { get; set; }

        [XmlElement("SITE_ID")]
        public string SiteId { get; set; }

        [XmlElement("SAMPLE_DEPTH")]
        public double SampleDepth { get; set; }

        [XmlElement("GAUGING_ID")]
        public string GaugingId { get; set; }

        [XmlIgnore]
        public IReadOnlyList<VerticalItem> Verticals { get; set; }
    }
}
