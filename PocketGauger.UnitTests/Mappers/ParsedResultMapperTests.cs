using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.Mappers
{
    [TestFixture]
    public class ParsedResultMapperTests
    {
        private IFixture _fixture;
        private IParseContext _parseContext;
        private ParseContextTestHelper _parseContextTestHelper;
        private ParsedResultMapper _mapper;

        private GaugingSummary _gaugingSummary;

        [SetUp]
        public void SetupForEachTest()
        {
            _fixture = new Fixture();
            _fixture.Customizations.Add(new ProxyTypeSpecimenBuilder());

            _parseContextTestHelper = new ParseContextTestHelper();
            _parseContext = _parseContextTestHelper.CreateMockParseContext();
            _mapper = new ParsedResultMapper(_parseContext, new DischargeActivityMapper(_parseContext));

            _gaugingSummary = _fixture.Build<GaugingSummary>()
                .With(summary => summary.GaugingSummaryItems, _fixture.CreateMany<GaugingSummaryItem>().ToList())
                .Create();
        }

        [Test]
        public void CreateParsedResults_SiteIdDoesNotMatchExistingLocation_Throws()
        {
            _parseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns((ILocationInfo)null);

            TestDelegate testDelegate = () => _mapper.CreateParsedResults(_gaugingSummary);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>());
        }

        [Test]
        public void CreateParsedResults_SiteIdMatchesExistingLocation_RetrievesFieldVisitsForThatLocation()
        {
            _mapper.CreateParsedResults(_gaugingSummary);

            _parseContextTestHelper.LocationInfo.Received()
                .FindLocationFieldVisitsInTimeRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void CreateParsedResults_IntersectingExistingVisit_ReturnsNewDischargeActivityForActivities()
        {
            var fieldVisitCoveringAllTime = Substitute.For<IFieldVisitInfo>();
            fieldVisitCoveringAllTime.StartDate.Returns(DateTimeOffset.MinValue);
            fieldVisitCoveringAllTime.EndDate.Returns(DateTimeOffset.MaxValue);
            SetUpParseContext(new[] { fieldVisitCoveringAllTime });

            var results = _mapper.CreateParsedResults(_gaugingSummary);

            Assert.That(results, Has.All.TypeOf<NewDischargeActivity>());
        }

        private void SetUpParseContext(IEnumerable<IFieldVisitInfo> fieldVisitInfo)
        {
            _parseContext = _parseContextTestHelper.CreateMockParseContext(fieldVisitInfo);
        }

        [Test]
        public void CreateParsedResults_GaugingSummaryWithValidMutlipleItems_CreatesParsedResultForEachItem()
        {
            var results = _mapper.CreateParsedResults(_gaugingSummary);

            Assert.That(results, Has.Count.EqualTo(_gaugingSummary.GaugingSummaryItems.Count));
        }

        [Test]
        public void CreateParsedResults_NoExistingVisit_ReturnsNewFieldVisitForActivities()
        {
            var results = _mapper.CreateParsedResults(_gaugingSummary);

            Assert.That(results, Has.All.TypeOf<NewFieldVisit>());
        }
    }
}
