using System;
using System.Linq;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.DataModel;
using FieldDataPluginFramework.DataModel.DischargeActivities;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    public class GaugingSummaryProcessorTests : PocketGaugerTestsBase
    {
        private GaugingSummaryProcessor _gaugingSummaryProcessor;

        [SetUp]
        public new void SetUp()
        {
            _gaugingSummaryProcessor = new GaugingSummaryProcessor(FieldDataResultsAppender, Logger);
        }

        [Test]
        public void ProcessGaugingSummary_ValidGaugingSummary_MapsObserverToParty()
        {
            var expectedNumberOfItems = 3;

            var startDate = Fixture.Create<DateTime>();
            var duration = Fixture.Create<TimeSpan>().Duration();
            var observer = Fixture.Create<string>();

            var gaugingSummaryItems = Fixture.Build<GaugingSummaryItem>()
                .OmitAutoProperties()
                .With(x => x.StartDate, startDate)
                .With(x => x.EndDate, startDate.Add(duration))
                .With(x => x.ObserversName, observer)
                .With(x => x.GaugingId)
                .With(x => x.Flow)
                .With(x => x.Area)
                .With(x => x.MeanVelocity)
                .With(x => x.PanelItems, new PanelItem[]{})
                .CreateMany(expectedNumberOfItems).ToList();

            var gaugingSummary = new GaugingSummary { GaugingSummaryItems = gaugingSummaryItems };

            _gaugingSummaryProcessor.ProcessGaugingSummary(gaugingSummary);

            FieldDataResultsAppender
                .Received(expectedNumberOfItems)
                .AddFieldVisit(Arg.Any<LocationInfo>(), Arg.Is<FieldVisitDetails>(x => x.Party == observer));

            FieldDataResultsAppender
                .Received(expectedNumberOfItems)
                .AddDischargeActivity(Arg.Any<FieldVisitInfo>(), Arg.Is<DischargeActivity>(x => x.Party == observer));
        }
    }
}
