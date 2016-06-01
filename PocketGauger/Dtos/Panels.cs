using System.Collections.Generic;
using System.Xml.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlRoot("GF_PANELS")]
    public class Panels
    {
        [XmlElement("GF_PANELSItem")]
        public List<PanelItem> PanelItems { get; set; }
    }
}
