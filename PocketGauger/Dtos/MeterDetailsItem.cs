using System.Collections.Generic;
using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class MeterDetailsItem
    {
        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlElement("METER_NUMBER")]
        public string MeterNumber { get; set; }

        [XmlElement("IMPELLER_NUMBER")]
        public string ImpellerNumber { get; set; }

        [XmlElement("DESCRIPTION")]
        public string Description { get; set; }

        [XmlIgnore]
        public MeterType? MeterType { get; set; }

        [XmlElement("METER_TYPE")]
        public string MeterTypeProxy
        {
            get { return MeterType.ToString(); }
            set { MeterType = EnumHelper.Map<MeterType>(value); }
        }

        [XmlIgnore]
        public IReadOnlyList<MeterCalibrationItem> Calibrations { get; set; }
    }
}
