using System.Xml.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class MeterCalibrationItem
    {
        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlElement("MINROTATIONSPEED")]
        public double? MinRotationSpeed { get; set; }

        [XmlElement("FACTOR")]
        public double Factor { get; set; }

        [XmlElement("CONSTANT")]
        public double Constant { get; set; }
    }
}
