using FieldDataPluginFramework;
using FieldDataPluginFramework.Context;
using FieldDataPluginFramework.Results;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessObjects.FieldDataPlugin.UnitTests;
using Server.TestHelpers.FieldVisitTestHelpers.TestHelpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    public abstract class PocketGaugerTestsBase
    {
        protected IFieldDataResultsAppender FieldDataResultsAppender;
        protected ILog Logger;

        protected IFixture Fixture;

        private LocationInfo _locationInfo;

        [SetUp]
        public void SetUp()
        {
            Fixture = new Fixture();

            Logger = new NullLog();
            FieldDataResultsAppender = Substitute.For<IFieldDataResultsAppender>();

            _locationInfo = LocationInfoHelper.GetTestLocationInfo(Fixture);

            FieldDataResultsAppender
                .GetLocationByUniqueId(Arg.Any<string>())
                .Returns(_locationInfo);

            FieldDataResultsAppender
                .GetLocationByIdentifier(Arg.Any<string>())
                .Returns(_locationInfo);
        }
    }
}
