using System.Collections.Generic;
using System.Xml.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlRoot("GF_METER_DETAILS")]
    public class MeterDetails
    {
        [XmlElement("GF_METER_DETAILSItem")]
        public List<MeterDetailsItem> MeterDetailsItems { get; set; }
    }
}
