using System;
using System.Xml.Serialization;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.Dtos
{
    [XmlType]
    public class GaugingSummaryItem
    {
        [XmlElement("SITE_ID")]
        public string SiteId { get; set; }

        [XmlIgnore]
        public DateTime StartDate { get; set; }

        [XmlElement("START_DATE")]
        public string StartDateProxy
        {
            get { return DateTimeHelper.Serialize(StartDate); }
            set { StartDate = DateTimeHelper.Parse(value); }
        }

        [XmlIgnore]
        public DateTime EndDate { get; set; }

        [XmlElement("END_DATE")]
        public string EndDateProxy 
        {
            get { return DateTimeHelper.Serialize(EndDate); }
            set { EndDate = DateTimeHelper.Parse(value); }
        }

        [XmlElement("OBSERVERS_NAME")]
        public string ObserversName { get; set; }

        [XmlElement("NO_OF_PANELS")]
        public int NumberOfPanels { get; set; }

        [XmlElement("START_STAGE")]
        public double StartStage { get; set; }

        [XmlElement("END_STAGE")]
        public double EndStage { get; set; }

        [XmlElement("MEAN_STAGE")]
        public double MeanStage { get; set; }

        [XmlIgnore]
        public RiverState RiverState { get; set; }

        [XmlElement("RIVER_STATE")]
        public int RiverStateProxy
        {
            get { return (int)RiverState; }
            set { RiverState = EnumHelper.Map<RiverState>(value); }
        }

        [XmlElement("AREA")]
        public double Area { get; set; }

        [XmlElement("MEAN_VELOCITY")]
        public double MeanVelocity { get; set; }

        [XmlElement("FLOW")]
        public double Flow { get; set; }

        [XmlIgnore]
        public GaugingMethod GaugingMethod { get; set; }

        [XmlElement("GAUGING_METHOD")]
        public int GaugingMethodProxy
        {
            get { return (int)GaugingMethod; }
            set { GaugingMethod = EnumHelper.Map<GaugingMethod>(value); }
        }

        [XmlIgnore]
        public EntryMethod EntryMethod { get; set; }

        [XmlElement("ENTRY_METHOD")]
        public int EntryMethodProxy
        {
            get { return (int)EntryMethod; }
            set { EntryMethod = EnumHelper.Map<EntryMethod>(value); }
        }

        [XmlIgnore]
        public FlowCalculationMethod FlowCalculationMethod { get; set; }

        [XmlElement("FLOW_CALC_METHOD")]
        public int FlowCalculationMethodProxy
        {
            get { return (int)FlowCalculationMethod; }
            set { FlowCalculationMethod = EnumHelper.Map<FlowCalculationMethod>(value); }
        }

        [XmlIgnore]
        public VelocityCalculationMethod VelocityCalculationMethod { get; set; }

        [XmlElement("VEL_CALC_METHOD")]
        public int VelocityCalculationMethodProxy 
        {
            get { return (int)VelocityCalculationMethod; }
            set { VelocityCalculationMethod = EnumHelper.Map<VelocityCalculationMethod>(value); }
        }

        [XmlIgnore]
        public MeterMethod CMeterMethod { get; set; }

        [XmlElement("CMETER_METHOD")]
        public int CMeterMethodProxy 
        {
            get { return (int)CMeterMethod; }
            set { CMeterMethod = EnumHelper.Map<MeterMethod>(value); }
        }

        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlElement("COUNTER_ID")]
        public string CounterId { get; set; }

        [XmlIgnore]
        public BankSide StartBank { get; set; }

        [XmlElement("START_BANK")]
        public int StartBankProxy 
        {
            get { return (int)RiverState; }
            set { RiverState = EnumHelper.Map<RiverState>(value); }
        }

        [XmlElement("RATING_ID")]
        public string RatingId { get; set; }

        [XmlElement("MULTIPLE_METER_NO")]
        public int MultipleMeterNo { get; set; }

        [XmlIgnore]
        public bool SampleAtSurface;

        [XmlElement("SAMPLE_AT_SURFACE")]
        public string SampleAtSurfaceProxy
        {
            get { return BooleanHelper.Serialize(SampleAtSurface); }
            set { SampleAtSurface = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt2;

        [XmlElement("SAMPLE_AT_2")]
        public string SampleAt2Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt2); }
            set { SampleAt2 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt4;

        [XmlElement("SAMPLE_AT_4")]
        public string SampleAt4Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt4); }
            set { SampleAt4 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt5;

        [XmlElement("SAMPLE_AT_5")]
        public string SampleAt5Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt5); }
            set { SampleAt5 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt6;

        [XmlElement("SAMPLE_AT_6")]
        public string SampleAt6Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt6); }
            set { SampleAt6 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt8;

        [XmlElement("SAMPLE_AT_8")]
        public string SampleAt8Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt8); }
            set { SampleAt8 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAtBed;

        [XmlElement("SAMPLE_AT_BED")]
        public string SampleAtBedProxy 
        {
            get { return BooleanHelper.Serialize(SampleAtBed); }
            set { SampleAtBed = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool IntegrationMethod;

        [XmlElement("INTEGRATION_METHOD")]
        public string IntegrationMethodProxy 
        {
            get { return BooleanHelper.Serialize(IntegrationMethod); }
            set { IntegrationMethod = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool DistribMethod;

        [XmlElement("DISTRIB_METHOD")]
        public string DistribMethodProxy
        {
            get { return BooleanHelper.Serialize(DistribMethod); }
            set { DistribMethod = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool WMeanStage;

        [XmlElement("W_MEAN_STAGE")]
        public string WMeanStageProxy
        {
            get { return BooleanHelper.Serialize(WMeanStage); }
            set { WMeanStage = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool NewSite;

        [XmlElement("NEW_SITE")]
        public string NewSiteProxy
        {
            get { return BooleanHelper.Serialize(NewSite); }
            set { NewSite = BooleanHelper.Parse(value); }
        }

        [XmlElement("SITE_NAME")]
        public string SiteName { get; set; }

        [XmlElement("SITE_LOCATION")]
        public string SiteLocation { get; set; }

        [XmlElement("RIVER_NAME")]
        public string RiverName { get; set; }

        [XmlElement("LOC_REF_NGR")]
        public string LocRefNgr { get; set; }

        [XmlElement("COMMENTS")]
        public string Comments { get; set; }

        [XmlElement("RATING_FLOW")]
        public double RatingFlow { get; set; }

        [XmlElement("RATING_DEVIATION")]
        public double RatingDeviation { get; set; }

        [XmlElement("GAUGING_ID")]
        public double GaugingId { get; set; }

        [XmlElement("GSTATUS")]
        public int GStatus { get; set; }

        [XmlElement("PRDEPTH")]
        public int PrDepth { get; set; }

        [XmlIgnore]
        public bool EndCorrect { get; set; }

        [XmlElement("ENDCORRECT")]
        public string EndCorrectProxy
        {
            get { return BooleanHelper.Serialize(EndCorrect); }
            set { EndCorrect = BooleanHelper.Parse(value); }
        }

        [XmlElement("CFACTOR1")]
        public double CFactor1 { get; set; }

        [XmlElement("CFACTOR2")]
        public double CFactor2 { get; set; }

        [XmlElement("INDEX_VELOCITY")]
        public double IndexVelocity { get; set; }

        [XmlIgnore]
        public bool UseIndexVelocity { get; set; }

        [XmlElement("USE_INDEX_VELOCITY")]
        public string UseIndexVelocityProxy
        {
            get { return BooleanHelper.Serialize(UseIndexVelocity); }
            set { UseIndexVelocity = BooleanHelper.Parse(value); }
        }
    }
}
