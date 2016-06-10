using System;
using System.Collections.Generic;
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

        [XmlIgnore]
        public int? NumberOfPanels { get; set; }

        [XmlElement("NO_OF_PANELS")]
        public string NumberOfPanelsProxy
        {
            get { return IntHelper.Serialize(NumberOfPanels); }
            set { NumberOfPanels = IntHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? StartStage { get; set; }

        [XmlElement("START_STAGE")]
        public string StartStageProxy
        {
            get { return DoubleHelper.Serialize(StartStage); }
            set { StartStage = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? EndStage { get; set; }

        [XmlElement("END_STAGE")]
        public string EndStageProxy
        {
            get { return DoubleHelper.Serialize(EndStage); }
            set { EndStage = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? MeanStage { get; set; }

        [XmlElement("MEAN_STAGE")]
        public string MeanStageProxy
        {
            get { return DoubleHelper.Serialize(MeanStage); }
            set { MeanStage = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public RiverState? RiverState { get; set; }

        [XmlElement("RIVER_STATE")]
        public string RiverStateProxy
        {
            get { return RiverState.ToString(); }
            set { RiverState = EnumHelper.Map<RiverState>(value); }
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
        public GaugingMethod? GaugingMethod { get; set; }

        [XmlElement("GAUGING_METHOD")]
        public string GaugingMethodProxy
        {
            get { return GaugingMethod.ToString(); }
            set { GaugingMethod = EnumHelper.Map<GaugingMethod>(value); }
        }

        [XmlIgnore]
        public EntryMethod? EntryMethod { get; set; }

        [XmlElement("ENTRY_METHOD")]
        public string EntryMethodProxy
        {
            get { return EntryMethod.ToString(); }
            set { EntryMethod = EnumHelper.Map<EntryMethod>(value); }
        }

        [XmlIgnore]
        public FlowCalculationMethod? FlowCalculationMethod { get; set; }

        [XmlElement("FLOW_CALC_METHOD")]
        public string FlowCalculationMethodProxy
        {
            get { return FlowCalculationMethod.ToString(); }
            set { FlowCalculationMethod = EnumHelper.Map<FlowCalculationMethod>(value); }
        }

        [XmlIgnore]
        public VelocityCalculationMethod? VelocityCalculationMethod { get; set; }

        [XmlElement("VEL_CALC_METHOD")]
        public string VelocityCalculationMethodProxy 
        {
            get { return VelocityCalculationMethod.ToString(); }
            set { VelocityCalculationMethod = EnumHelper.Map<VelocityCalculationMethod>(value); }
        }

        [XmlIgnore]
        public MeterMethod? CMeterMethod { get; set; }

        [XmlElement("CMETER_METHOD")]
        public string CMeterMethodProxy 
        {
            get { return CMeterMethod.ToString(); }
            set { CMeterMethod = EnumHelper.Map<MeterMethod>(value); }
        }

        [XmlElement("METER_ID")]
        public string MeterId { get; set; }

        [XmlElement("COUNTER_ID")]
        public string CounterId { get; set; }

        [XmlIgnore]
        public BankSide? StartBank { get; set; }

        [XmlElement("START_BANK")]
        public string StartBankProxy 
        {
            get { return StartBank.ToString(); }
            set { StartBank = EnumHelper.Map<BankSide>(value); }
        }

        [XmlElement("RATING_ID")]
        public string RatingId { get; set; }

        [XmlIgnore]
        public int? MultipleMeterNo { get; set; }

        [XmlElement("MULTIPLE_METER_NO")]
        public string MultipleMeterNoProxy
        {
            get { return IntHelper.Serialize(MultipleMeterNo); }
            set { MultipleMeterNo = IntHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAtSurface { get; set; }

        [XmlElement("SAMPLE_AT_SURFACE")]
        public string SampleAtSurfaceProxy
        {
            get { return BooleanHelper.Serialize(SampleAtSurface); }
            set { SampleAtSurface = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt2 { get; set; }

        [XmlElement("SAMPLE_AT_2")]
        public string SampleAt2Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt2); }
            set { SampleAt2 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt4 { get; set; }

        [XmlElement("SAMPLE_AT_4")]
        public string SampleAt4Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt4); }
            set { SampleAt4 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt5 { get; set; }

        [XmlElement("SAMPLE_AT_5")]
        public string SampleAt5Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt5); }
            set { SampleAt5 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt6 { get; set; }

        [XmlElement("SAMPLE_AT_6")]
        public string SampleAt6Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt6); }
            set { SampleAt6 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAt8 { get; set; }

        [XmlElement("SAMPLE_AT_8")]
        public string SampleAt8Proxy
        {
            get { return BooleanHelper.Serialize(SampleAt8); }
            set { SampleAt8 = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool SampleAtBed { get; set; }

        [XmlElement("SAMPLE_AT_BED")]
        public string SampleAtBedProxy 
        {
            get { return BooleanHelper.Serialize(SampleAtBed); }
            set { SampleAtBed = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool IntegrationMethod { get; set; }

        [XmlElement("INTEGRATION_METHOD")]
        public string IntegrationMethodProxy 
        {
            get { return BooleanHelper.Serialize(IntegrationMethod); }
            set { IntegrationMethod = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool DistribMethod { get; set; }

        [XmlElement("DISTRIB_METHOD")]
        public string DistribMethodProxy
        {
            get { return BooleanHelper.Serialize(DistribMethod); }
            set { DistribMethod = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool WMeanStage { get; set; }

        [XmlElement("W_MEAN_STAGE")]
        public string WMeanStageProxy
        {
            get { return BooleanHelper.Serialize(WMeanStage); }
            set { WMeanStage = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool NewSite { get; set; }

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

        [XmlIgnore]
        public double? RatingFlow { get; set; }

        [XmlElement("RATING_FLOW")]
        public string RatingFlowProxy
        {
            get { return DoubleHelper.Serialize(RatingFlow); }
            set { RatingFlow = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? RatingDeviation { get; set; }

        [XmlElement("RATING_DEVIATION")]
        public string RatingDeviationProxy
        {
            get { return DoubleHelper.Serialize(RatingDeviation); }
            set { RatingDeviation = DoubleHelper.Parse(value); }
        }

        [XmlElement("GAUGING_ID")]
        public string GaugingId { get; set; }

        [XmlIgnore]
        public int? GStatus { get; set; }

        [XmlElement("GSTATUS")]
        public string GStatusProxy
        {
            get { return IntHelper.Serialize(GStatus); }
            set { GStatus = IntHelper.Parse(value); }
        }

        [XmlIgnore]
        public int? PrDepth { get; set; }

        [XmlElement("PRDEPTH")]
        public string PrDepthProxy
        {
            get { return IntHelper.Serialize(PrDepth); }
            set { PrDepth = IntHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool EndCorrect { get; set; }

        [XmlElement("ENDCORRECT")]
        public string EndCorrectProxy
        {
            get { return BooleanHelper.Serialize(EndCorrect); }
            set { EndCorrect = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? CFactor1 { get; set; }

        [XmlElement("CFACTOR1")]
        public string CFactor1Proxy
        {
            get { return DoubleHelper.Serialize(CFactor1); }
            set { CFactor1 = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? CFactor2 { get; set; }

        [XmlElement("CFACTOR2")]
        public string CFactor2Proxy
        {
            get { return DoubleHelper.Serialize(CFactor2); }
            set { CFactor2 = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public double? IndexVelocity { get; set; }

        [XmlElement("INDEX_VELOCITY")]
        public string IndexVelocityProxy
        {
            get { return DoubleHelper.Serialize(IndexVelocity); }
            set { IndexVelocity = DoubleHelper.Parse(value); }
        }

        [XmlIgnore]
        public bool UseIndexVelocity { get; set; }

        [XmlElement("USE_INDEX_VELOCITY")]
        public string UseIndexVelocityProxy
        {
            get { return BooleanHelper.Serialize(UseIndexVelocity); }
            set { UseIndexVelocity = BooleanHelper.Parse(value); }
        }

        [XmlIgnore]
        public MeterDetailsItem MeterDetailsItem { get; set; }

        [XmlIgnore]
        public IReadOnlyCollection<PanelItem> PanelItems { get; set; }
    }
}
