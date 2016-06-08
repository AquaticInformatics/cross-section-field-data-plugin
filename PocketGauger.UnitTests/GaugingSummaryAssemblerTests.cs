using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    public class GaugingSummaryAssemblerTests
    {
        private IFixture _fixture;
        private IGaugingSummaryParser _gaugingSummaryParser;
        private IMeterDetailsParser _meterDetailsParser;
        private IPanelParser _panelParser;
        private PocketGaugerFiles _pocketGaugerFiles;
        private GaugingSummaryAssembler _gaugingSummaryAssembler;

        private readonly string[] _meterIds = {"1", "2", "3"};

        [SetUp]
        public void SetUp()
        {
            SetUpAutoFixture();

            SetUpGaugingSummaryParser();
            SetUpMeterDetailsParser();
            _panelParser = Substitute.For<IPanelParser>();

            _pocketGaugerFiles = new PocketGaugerFiles();

            _gaugingSummaryAssembler = new GaugingSummaryAssembler(_gaugingSummaryParser, _meterDetailsParser,
                _panelParser);
        }

        private void SetUpAutoFixture()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());
            CollectionRegistrar.Register(_fixture);
        }

        private void SetUpMeterDetailsParser()
        {
            _meterDetailsParser = Substitute.For<IMeterDetailsParser>();

            var meterDetailsItems = _fixture.CreateMany<MeterDetailsItem>(3).ToList();
            for (var i = 0; i < meterDetailsItems.Count; i++)
            {
                meterDetailsItems[i].MeterId = _meterIds[i];
            }
            var meterItemsDictionary = meterDetailsItems.ToDictionary(m => m.MeterId, m => m);

            _meterDetailsParser.Parse(Arg.Any<PocketGaugerFiles>()).Returns(meterItemsDictionary);
        }

        private void SetUpGaugingSummaryParser()
        {
            _gaugingSummaryParser = Substitute.For<IGaugingSummaryParser>();

            var gaugingSummaryItems =
                _fixture.Build<GaugingSummaryItem>()
                    .Without(g => g.MeterDetailsItem)
                    .Without(g => g.PanelItems)
                    .CreateMany(3)
                    .ToList();
            for (var i = 0; i < gaugingSummaryItems.Count; i++)
            {
                gaugingSummaryItems[i].MeterId = _meterIds[i];
            }
            var gaugingSummary = new GaugingSummary {GaugingSummaryItems = gaugingSummaryItems};

            _gaugingSummaryParser.Parse(Arg.Any<PocketGaugerFiles>()).Returns(gaugingSummary);
        }

        [Test]
        public void Assemble_AttachesMeterDetailAccordingToMeterId()
        {
            var result = _gaugingSummaryAssembler.Assemble(_pocketGaugerFiles);

            foreach (var gaugingSummaryItem in result.GaugingSummaryItems)
            {
                Assert.That(gaugingSummaryItem.MeterDetailsItem.MeterId == gaugingSummaryItem.MeterId);
            }
        }

        [Test]
        public void Assemble_AttachesPanelItemsAccordingToGaugingId()
        {
            var result = _gaugingSummaryAssembler.Assemble(_pocketGaugerFiles);

            foreach (var gaugingSummaryItem in result.GaugingSummaryItems)
            {
                Assert.That(gaugingSummaryItem.PanelItems,
                    Has.All.Matches<PanelItem>(p => p.GaugingId == gaugingSummaryItem.GaugingId));
            }
        }
    }
}
