using System;
using NUnit.Framework;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.IntegrationTests
{
    public class GaugingSummaryParserTests : IntegrationTestBase
    {
        [Test]
        public void Parse_PocketGaugerFilesContainsValidGaugingSummary_ReturnsExpectedDto()
        {
            AddPocketGaugerFile(FileNames.GaugingSummary);

            var result = GaugingSummaryParser.Parse(PocketGaugerFiles);

            ValidateItem0(result.GaugingSummaryItems[0]);
            ValidateItem1(result.GaugingSummaryItems[1]);
            ValidateItem2(result.GaugingSummaryItems[2]);
        }

        private static void ValidateItem0(GaugingSummaryItem gaugingSummaryItem)
        {
            Assert.That(gaugingSummaryItem.SiteId, Is.EqualTo("DEMO3"));
            Assert.That(gaugingSummaryItem.StartDate, Is.EqualTo(new DateTime(2004, 12, 31, 11, 15, 1)));
            Assert.That(gaugingSummaryItem.EndDate, Is.EqualTo(new DateTime(2004, 12, 31, 11, 18, 45)));
            Assert.That(gaugingSummaryItem.ObserversName, Is.EqualTo("Fred"));
            Assert.That(gaugingSummaryItem.NumberOfPanels, Is.EqualTo(5));
            Assert.That(gaugingSummaryItem.StartStage, Is.EqualTo(0.2));
            Assert.That(gaugingSummaryItem.EndStage, Is.EqualTo(0.25));
            Assert.That(gaugingSummaryItem.MeanStage, Is.EqualTo(0.225));
            Assert.That(gaugingSummaryItem.RiverState, Is.EqualTo(RiverState.Rising));
            Assert.That(gaugingSummaryItem.Area, Is.EqualTo(10));
            Assert.That(gaugingSummaryItem.MeanVelocity, Is.EqualTo(2.5));
            Assert.That(gaugingSummaryItem.Flow, Is.EqualTo(25));
            Assert.That(gaugingSummaryItem.VelocityCalculationMethod, Is.EqualTo(VelocityCalculationMethod.Arithmetic));
            Assert.That(gaugingSummaryItem.CMeterMethod, Is.EqualTo(MeterMethod.Single));
            Assert.That(gaugingSummaryItem.MeterId, Is.EqualTo("12345±"));
            Assert.That(gaugingSummaryItem.CounterId, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.StartBank, Is.EqualTo(BankSide.Left));
            Assert.That(gaugingSummaryItem.RatingId, Is.EqualTo("DEMO3±12±4"));
            Assert.That(gaugingSummaryItem.MultipleMeterNo, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.SampleAtSurface, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt2, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt4, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt5, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt6, Is.EqualTo(true));
            Assert.That(gaugingSummaryItem.SampleAt8, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAtBed, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.IntegrationMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.DistribMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.WMeanStage, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.NewSite, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SiteName, Is.EqualTo("Kingsfield House"));
            Assert.That(gaugingSummaryItem.SiteLocation, Is.EqualTo("Butts Bridge"));
            Assert.That(gaugingSummaryItem.RiverName, Is.EqualTo("River Lugg"));
            Assert.That(gaugingSummaryItem.LocRefNgr, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.Comments, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.RatingFlow, Is.EqualTo(0.815922819921845));
            Assert.That(gaugingSummaryItem.RatingDeviation, Is.EqualTo(-96.7363087203126));
            Assert.That(gaugingSummaryItem.GaugingId, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.GStatus, Is.EqualTo(2));
            Assert.That(gaugingSummaryItem.PrDepth, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.EndCorrect, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.CFactor1, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.CFactor2, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.IndexVelocity, Is.EqualTo(1.5));
            Assert.That(gaugingSummaryItem.UseIndexVelocity, Is.EqualTo(true));
        }

        private static void ValidateItem1(GaugingSummaryItem gaugingSummaryItem)
        {
            Assert.That(gaugingSummaryItem.SiteId, Is.EqualTo("DEMO4"));
            Assert.That(gaugingSummaryItem.StartDate, Is.EqualTo(new DateTime(2004, 12, 31, 14, 35, 41)));
            Assert.That(gaugingSummaryItem.EndDate, Is.EqualTo(new DateTime(2004, 12, 31, 14, 36, 24)));
            Assert.That(gaugingSummaryItem.ObserversName, Is.EqualTo("Fred"));
            Assert.That(gaugingSummaryItem.NumberOfPanels, Is.EqualTo(5));
            Assert.That(gaugingSummaryItem.StartStage, Is.EqualTo(0.2));
            Assert.That(gaugingSummaryItem.EndStage, Is.EqualTo(0.25));
            Assert.That(gaugingSummaryItem.MeanStage, Is.EqualTo(0.225));
            Assert.That(gaugingSummaryItem.RiverState, Is.EqualTo(RiverState.Rising));
            Assert.That(gaugingSummaryItem.Area, Is.EqualTo(10));
            Assert.That(gaugingSummaryItem.MeanVelocity, Is.EqualTo(2.5));
            Assert.That(gaugingSummaryItem.Flow, Is.EqualTo(25));
            Assert.That(gaugingSummaryItem.VelocityCalculationMethod, Is.EqualTo(VelocityCalculationMethod.Arithmetic));
            Assert.That(gaugingSummaryItem.CMeterMethod, Is.EqualTo(MeterMethod.Single));
            Assert.That(gaugingSummaryItem.MeterId, Is.EqualTo("12345±"));
            Assert.That(gaugingSummaryItem.CounterId, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.StartBank, Is.EqualTo(BankSide.Left));
            Assert.That(gaugingSummaryItem.RatingId, Is.EqualTo("DEMO4±NEW±1"));
            Assert.That(gaugingSummaryItem.MultipleMeterNo, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.SampleAtSurface, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt2, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt4, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt5, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt6, Is.EqualTo(true));
            Assert.That(gaugingSummaryItem.SampleAt8, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAtBed, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.IntegrationMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.DistribMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.WMeanStage, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.NewSite, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SiteName, Is.EqualTo("Oak Tree House"));
            Assert.That(gaugingSummaryItem.SiteLocation, Is.EqualTo("Newtown"));
            Assert.That(gaugingSummaryItem.RiverName, Is.EqualTo("The Avon"));
            Assert.That(gaugingSummaryItem.LocRefNgr, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.Comments, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.RatingFlow, Is.EqualTo(11.6018483052899));
            Assert.That(gaugingSummaryItem.RatingDeviation, Is.EqualTo(-53.5926067788405));
            Assert.That(gaugingSummaryItem.GaugingId, Is.EqualTo(2));
            Assert.That(gaugingSummaryItem.GStatus, Is.EqualTo(2));
            Assert.That(gaugingSummaryItem.PrDepth, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.EndCorrect, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.CFactor1, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.CFactor2, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.IndexVelocity, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.UseIndexVelocity, Is.EqualTo(false));
        }

        private static void ValidateItem2(GaugingSummaryItem gaugingSummaryItem)
        {
            Assert.That(gaugingSummaryItem.SiteId, Is.EqualTo("DEMO5"));
            Assert.That(gaugingSummaryItem.StartDate, Is.EqualTo(new DateTime(2005, 1, 4, 12, 21, 31)));
            Assert.That(gaugingSummaryItem.EndDate, Is.EqualTo(new DateTime(2005, 1, 4, 12, 23, 5)));
            Assert.That(gaugingSummaryItem.ObserversName, Is.EqualTo("Fred"));
            Assert.That(gaugingSummaryItem.NumberOfPanels, Is.EqualTo(5));
            Assert.That(gaugingSummaryItem.StartStage, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.EndStage, Is.EqualTo(2));
            Assert.That(gaugingSummaryItem.MeanStage, Is.EqualTo(1.5));
            Assert.That(gaugingSummaryItem.RiverState, Is.EqualTo(RiverState.Rising));
            Assert.That(gaugingSummaryItem.Area, Is.EqualTo(3));
            Assert.That(gaugingSummaryItem.MeanVelocity, Is.EqualTo(2.33333333333333));
            Assert.That(gaugingSummaryItem.Flow, Is.EqualTo(7));
            Assert.That(gaugingSummaryItem.VelocityCalculationMethod, Is.EqualTo(VelocityCalculationMethod.Arithmetic));
            Assert.That(gaugingSummaryItem.CMeterMethod, Is.EqualTo(MeterMethod.Single));
            Assert.That(gaugingSummaryItem.MeterId, Is.EqualTo("12345±"));
            Assert.That(gaugingSummaryItem.CounterId, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.StartBank, Is.EqualTo(BankSide.Left));
            Assert.That(gaugingSummaryItem.RatingId, Is.EqualTo("DEMO5±TEST±2"));
            Assert.That(gaugingSummaryItem.MultipleMeterNo, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.SampleAtSurface, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt2, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt4, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt5, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAt6, Is.EqualTo(true));
            Assert.That(gaugingSummaryItem.SampleAt8, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SampleAtBed, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.IntegrationMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.DistribMethod, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.WMeanStage, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.NewSite, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.SiteName, Is.EqualTo("Butford Farm"));
            Assert.That(gaugingSummaryItem.SiteLocation, Is.EqualTo("Bowley Lane"));
            Assert.That(gaugingSummaryItem.RiverName, Is.EqualTo("River Stour"));
            Assert.That(gaugingSummaryItem.LocRefNgr, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.Comments, Is.EqualTo(string.Empty));
            Assert.That(gaugingSummaryItem.RatingFlow, Is.EqualTo(152.16121130221401));
            Assert.That(gaugingSummaryItem.RatingDeviation, Is.EqualTo(2073.7315900316298));
            Assert.That(gaugingSummaryItem.GaugingId, Is.EqualTo(7));
            Assert.That(gaugingSummaryItem.GStatus, Is.EqualTo(2));
            Assert.That(gaugingSummaryItem.PrDepth, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.EndCorrect, Is.EqualTo(false));
            Assert.That(gaugingSummaryItem.CFactor1, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.CFactor2, Is.EqualTo(1));
            Assert.That(gaugingSummaryItem.IndexVelocity, Is.EqualTo(0));
            Assert.That(gaugingSummaryItem.UseIndexVelocity, Is.EqualTo(true));
        }
    }
}
