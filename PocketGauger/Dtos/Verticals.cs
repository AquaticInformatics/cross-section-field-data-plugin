using System.Collections.Generic;
using System.Xml.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlRoot("GF_VERTICALS")]
    public class Verticals
    {
        [XmlElement("GF_VERTICALSItem")]
        public List<VerticalItem> VerticalItems { get; set; }
    }
}
