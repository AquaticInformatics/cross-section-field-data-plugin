using System.Collections.Generic;
using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class PanelItem
    {
        [XmlElement("PANEL_ID")]
        public string PanelId { get; set; }

        [XmlIgnore]
        public double? Stage { get; set; }

        [XmlElement("STAGE")]
        public string StageProxy
        {
            get { return DoubleHelper.Serialize(Stage); }
            set { Stage = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Area { get; set; }

        [XmlElement("AREA")]
        public string AreaProxy
        {
            get { return DoubleHelper.Serialize(Area); }
            set { Area = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? MeanVelocity { get; set; }

        [XmlElement("MEAN_VELOCITY")]
        public string MeanVelocityProxy
        {
            get { return DoubleHelper.Serialize(MeanVelocity); }
            set { MeanVelocity = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Flow { get; set; }

        [XmlElement("FLOW")]
        public string FlowProxy
        {
            get { return DoubleHelper.Serialize(Flow); }
            set { Flow = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public int? VerticalNumber { get; set; }

        [XmlElement("VERTICAL_NUMBER")]
        public string VerticalNumberProxy
        {
            get { return IntHelper.Serialize(VerticalNumber); }
            set { VerticalNumber = IntHelper.Parse(value); }
        }

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

        [XmlElement("SITE_ID")]
        public string SiteId { get; set; }

        [XmlIgnore]
        public double? SampleDepth { get; set; }

        [XmlElement("SAMPLE_DEPTH")]
        public string SampleDepthProxy
        {
            get { return DoubleHelper.Serialize(SampleDepth); }
            set { SampleDepth = DoubleHelper.Parse(value); }
        }

        [XmlElement("GAUGING_ID")]
        public string GaugingId { get; set; }

        [XmlIgnore]
        public IReadOnlyList<VerticalItem> Verticals { get; set; }
    }
}
