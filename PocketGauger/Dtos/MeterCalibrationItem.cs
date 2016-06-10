using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class MeterCalibrationItem
    {
        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlIgnore]
        public double? MinRotationSpeed { get; set; }

        [XmlElement("MINROTATIONSPEED")]
        public string MinRotationSpeedProxy
        {
            get { return DoubleHelper.Serialize(MinRotationSpeed); }
            set { MinRotationSpeed = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Factor { get; set; }

        [XmlElement("FACTOR")]
        public string FactorProxy
        {
            get { return DoubleHelper.Serialize(Factor); }
            set { Factor = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? Constant { get; set; }

        [XmlElement("CONSTANT")]
        public string ConstantProxy
        {
            get { return DoubleHelper.Serialize(Constant); }
            set { Constant = DoubleHelper.Parse(value); }
        }
    }
}
