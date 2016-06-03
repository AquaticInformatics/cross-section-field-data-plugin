using System;
using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public static class ExpectedGaugingSummaryData
    {
        public static GaugingSummary CreateExpectedGaugingSummary()
        {
            return new GaugingSummary
            {
                GaugingSummaryItems = new List<GaugingSummaryItem>
                {
                    CreateGaugingSummaryItem0(),
                    CreateGaugingSummaryItem1(),
                    CreateGaugingSummaryItem2()
                }
            };
        }

        private static GaugingSummaryItem CreateGaugingSummaryItem0()
        {
            return new GaugingSummaryItem
            {
                SiteId = "DEMO3",
                StartDate = new DateTime(2004, 12, 31, 11, 15, 1),
                EndDate = new DateTime(2004, 12, 31, 11, 18, 45),
                ObserversName = "Fred",
                NumberOfPanels = 5,
                StartStage = 0.2,
                EndStage = 0.25,
                MeanStage = 0.225,
                RiverState = RiverState.Rising,
                Area = 10,
                MeanVelocity = 2.5,
                Flow = 25,
                GaugingMethod = GaugingMethod.Waded,
                GaugingMethodProxy = "0",
                EntryMethod = EntryMethod.GaugerField,
                FlowCalculationMethod = FlowCalculationMethod.Mean,
                FlowCalculationMethodProxy = "0",
                VelocityCalculationMethod = VelocityCalculationMethod.Arithmetic,
                CMeterMethod = MeterMethod.Single,
                MeterId = "12345±",
                CounterId = string.Empty,
                StartBank = BankSide.Left,
                RatingId = "DEMO3±12±4",
                MultipleMeterNo = 0,
                SampleAtSurface = false,
                SampleAt2 = false,
                SampleAt4 = false,
                SampleAt5 = false,
                SampleAt6 = true,
                SampleAt8 = false,
                SampleAtBed = false,
                IntegrationMethod = false,
                DistribMethod = false,
                WMeanStage = false,
                NewSite = false,
                SiteName = "Kingsfield House",
                SiteLocation = "Butts Bridge",
                RiverName = "River Lugg",
                LocRefNgr = string.Empty,
                Comments = string.Empty,
                RatingFlow = 0.815922819921845,
                RatingDeviation = -96.7363087203126,
                GaugingId = "1",
                GStatus = 2,
                PrDepth = 0,
                EndCorrect = false,
                CFactor1 = 1,
                CFactor2 = 1,
                IndexVelocity = 1.5,
                UseIndexVelocity = true,
            };
        }

        private static GaugingSummaryItem CreateGaugingSummaryItem1()
        {
            return new GaugingSummaryItem
            {
                SiteId = "DEMO4",
                StartDate = new DateTime(2004, 12, 31, 14, 35, 41),
                EndDate = new DateTime(2004, 12, 31, 14, 36, 24),
                ObserversName = "Fred",
                NumberOfPanels = 5,
                StartStage = 0.2,
                EndStage = 0.25,
                MeanStage = 0.225,
                RiverState = RiverState.Rising,
                Area = 10,
                MeanVelocity = 2.5,
                Flow = 25,
                GaugingMethod = GaugingMethod.Waded,
                GaugingMethodProxy = "0",
                EntryMethod = EntryMethod.GaugerField,
                FlowCalculationMethod = FlowCalculationMethod.Mean,
                FlowCalculationMethodProxy = "0",
                VelocityCalculationMethod = VelocityCalculationMethod.Arithmetic,
                CMeterMethod = MeterMethod.Single,
                MeterId = "12345±",
                CounterId = string.Empty,
                StartBank = BankSide.Left,
                RatingId = "DEMO4±NEW±1",
                MultipleMeterNo = 0,
                SampleAtSurface = false,
                SampleAt2 = false,
                SampleAt4 = false,
                SampleAt5 = false,
                SampleAt6 = true,
                SampleAt8 = false,
                SampleAtBed = false,
                IntegrationMethod = false,
                DistribMethod = false,
                WMeanStage = false,
                NewSite = false,
                SiteName = "Oak Tree House",
                SiteLocation = "Newtown",
                RiverName = "The Avon",
                LocRefNgr = string.Empty,
                Comments = string.Empty,
                RatingFlow = 11.6018483052899,
                RatingDeviation = -53.5926067788405,
                GaugingId = "2",
                GStatus = 2,
                PrDepth = 0,
                EndCorrect = false,
                CFactor1 = 1,
                CFactor2 = 1,
                IndexVelocity = 0,
                UseIndexVelocity = false,
                UseIndexVelocityProxy = "False"
            };
        }

        private static GaugingSummaryItem CreateGaugingSummaryItem2()
        {
            return new GaugingSummaryItem
            {
                SiteId = "DEMO5",
                StartDate = new DateTime(2005, 1, 4, 12, 21, 31),
                EndDate = new DateTime(2005, 1, 4, 12, 23, 5),
                ObserversName = "Fred",
                NumberOfPanels = 5,
                StartStage = 1,
                EndStage = 2,
                MeanStage = 1.5,
                RiverState = RiverState.Rising,
                Area = 3,
                MeanVelocity = 2.33333333333333,
                Flow = 7,
                GaugingMethod = GaugingMethod.Waded,
                GaugingMethodProxy = "0",
                EntryMethod = EntryMethod.GaugerField,
                FlowCalculationMethod = FlowCalculationMethod.Mean,
                FlowCalculationMethodProxy = "0",
                VelocityCalculationMethod = VelocityCalculationMethod.Arithmetic,
                CMeterMethod = MeterMethod.Single,
                MeterId = "12345±",
                CounterId = string.Empty,
                StartBank = BankSide.Left,
                RatingId = "DEMO5±TEST±2",
                MultipleMeterNo = 0,
                SampleAtSurface = false,
                SampleAt2 = false,
                SampleAt4 = false,
                SampleAt5 = false,
                SampleAt6 = true,
                SampleAt8 = false,
                SampleAtBed = false,
                IntegrationMethod = false,
                DistribMethod = false,
                WMeanStage = false,
                NewSite = false,
                SiteName = "Butford Farm",
                SiteLocation = "Bowley Lane",
                RiverName = "River Stour",
                LocRefNgr = string.Empty,
                Comments = string.Empty,
                RatingFlow = 152.16121130221401,
                RatingDeviation = 2073.7315900316298,
                GaugingId = "7",
                GStatus = 2,
                PrDepth = 0,
                EndCorrect = false,
                CFactor1 = 1,
                CFactor2 = 1,
                IndexVelocity = 0,
                UseIndexVelocity = true,
            };
        }
    }
}
