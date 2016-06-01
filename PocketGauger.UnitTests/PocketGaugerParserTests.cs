using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.TestHelpers.NUnitExtensions;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests
{
    [TestFixture]
    [LongRunning]
    public class PocketGaugerParserTests
    {
        private PocketGaugerParser _pocketGaugerParser;

        private Stream _stream;
        private IParseContext _parseContext;
        private ILog _logger;
        private ILocationInfo _locationInfo;
        private IParameter _dischargeParameter;
        private IParameter _gageHeightParameter;

        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _pocketGaugerParser = new PocketGaugerParser();
            _logger = null;
            SetUpParseContext();

            const string testPath = @"Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData.PGData.zip";
            _stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(testPath);
        }

        private void SetUpParseContext()
        {
            SetUpParseContext(new IFieldVisitInfo[] {});
        }

        private void SetUpParseContext(IEnumerable<IFieldVisitInfo> fieldVisitsToReturn)
        {
            _parseContext = Substitute.For<IParseContext>();

            SetUpLocation(fieldVisitsToReturn);
            SetUpDischargeParameter();
            SetUpGageHeightParameter();
            SetUpOtherParameters();
        }

        private void SetUpLocation(IEnumerable<IFieldVisitInfo> fieldVisitsToReturn)
        {
            _locationInfo = Substitute.For<ILocationInfo>();
            _locationInfo.FindLocationFieldVisitsInTimeRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                .Returns(fieldVisitsToReturn);

            var channel = Substitute.For<IChannelInfo>();
            _locationInfo.Channels.ReturnsForAnyArgs(new List<IChannelInfo> { channel });
            _parseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns(_locationInfo);
        }

        private void SetUpDischargeParameter()
        {
            _dischargeParameter = Substitute.For<IParameter>();

            var defaultDischargeUnit = Substitute.For<IUnit>();
            _dischargeParameter.DefaultUnit.Returns(defaultDischargeUnit);

            var midSectionMethod = Substitute.For<IMonitoringMethod>();
            midSectionMethod.MethodCode.Returns(ParametersAndMethodsConstants.MidSectionMonitoringMethod);
            var meanSectionMethod = Substitute.For<IMonitoringMethod>();
            meanSectionMethod.MethodCode.Returns(ParametersAndMethodsConstants.MeanSectionMonitoringMethod);
            var defaultDischargeMethod = Substitute.For<IMonitoringMethod>();
            defaultDischargeMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);

            _dischargeParameter.MonitoringMethods.Returns(new[]
                {midSectionMethod, meanSectionMethod, defaultDischargeMethod});

            _parseContext.DischargeParameter.Returns(_dischargeParameter);
        }

        private void SetUpGageHeightParameter()
        {
            _gageHeightParameter = Substitute.For<IParameter>();

            var defaultGageHeightUnit = Substitute.For<IUnit>();
            _dischargeParameter.DefaultUnit.Returns(defaultGageHeightUnit);

            var defaultGageHeightMethod = Substitute.For<IMonitoringMethod>();
            defaultGageHeightMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);
            _gageHeightParameter.MonitoringMethods.Returns(new List<IMonitoringMethod> {defaultGageHeightMethod});

            _parseContext.GageHeightParameter.Returns(_gageHeightParameter);
        }

        private void SetUpOtherParameters()
        {
            var parameters = new List<IParameter>
            {
                CreateMockParameter(ParametersAndMethodsConstants.AreaParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.VelocityParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.DistanceToGageParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.WidthParameterId)
            };

            _parseContext.AllParameters.ReturnsForAnyArgs(parameters);
        }

        private static IParameter CreateMockParameter(string parameterId)
        {
            var parameter = Substitute.For<IParameter>();
            parameter.Id.Returns(parameterId);

            var defaultUnit = Substitute.For<IUnit>();
            parameter.DefaultUnit.Returns(defaultUnit);

            var defaultGageHeightMethod = Substitute.For<IMonitoringMethod>();
            defaultGageHeightMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);
            parameter.MonitoringMethods.Returns(new List<IMonitoringMethod> { defaultGageHeightMethod });

            return parameter;
        }

        [TearDown]
        public void TearDown()
        {
            _stream.Close();
            _stream.Dispose();
        }

        [Test]
        public void ParseFile_FileStreamIsNotAValidZipFile_Throws()
        {
            _stream = new MemoryStream(_fixture.Create<byte[]>());

            TestDelegate testDelegate =
                () => _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>().With.Message.Contains("not a zip file"));
        }

        [Test]
        public void ParseFile_FileStreamZipDoesNotContainGaugingSummary_Throws()
        {
            _stream = CreateZipStream(_fixture.Create<string>());

            TestDelegate testDelegate = () => _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(testDelegate,
                Throws.Exception.TypeOf<FormatNotSupportedException>()
                    .With.Message.Contains(Invariant($"does not contain file {FileNames.GaugingSummary}")));
        }

        private Stream CreateZipStream(string zipEntryName)
        {
            var memoryStream = new MemoryStream();
            var zipOutputStream = new ZipOutputStream(memoryStream);
            AddZipEntry(zipOutputStream, zipEntryName);

            zipOutputStream.IsStreamOwner = false;
            zipOutputStream.Close();
            memoryStream.Position = 0;

            return memoryStream;
        }

        private void AddZipEntry(ZipOutputStream zipOutputStream, string zipEntryName)
        {
            var zipEntry = new ZipEntry(zipEntryName);
            zipOutputStream.PutNextEntry(zipEntry);

            const int entrySize = 4096;
            var sourceData = _fixture.CreateMany<byte>(entrySize).ToArray();
            var buffer = new byte[4096];
            StreamUtils.Copy(new MemoryStream(sourceData), zipOutputStream, buffer);
            zipOutputStream.CloseEntry();
        }

        [Test]
        public void ParseFile_ValidFileStreamZip_ReturnsNewFieldVisitForActivitiesNotIntersectingExistingVisit()
        {
            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(results, Has.All.TypeOf<NewFieldVisit>());
        }

        [Test]
        public void ParseFile_ValidFileStreamZip_ReturnsNewDischargeActivitiyForActivitiesIntersectingExistingVisit()
        {
            var fieldVisitCoveringAllTime = Substitute.For<IFieldVisitInfo>();
            fieldVisitCoveringAllTime.StartDate.Returns(DateTimeOffset.MinValue);
            fieldVisitCoveringAllTime.EndDate.Returns(DateTimeOffset.MaxValue);
            SetUpParseContext(new []{fieldVisitCoveringAllTime});

            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(results, Has.All.TypeOf<NewDischargeActivity>());
        }

        [Test]
        public void ParseFile_ReturnsResultsWithDischargeValuesSetAccordingToDischargeParameter()
        {
            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            var dischargeActivityResults = GetAllResultDischargeActivities(results);
            foreach (var dischargeActivity in dischargeActivityResults)
            {
                Assert.That(dischargeActivity.DischargeUnit, Is.EqualTo(_dischargeParameter.DefaultUnit));
                Assert.That(dischargeActivity.DischargeMethod.MethodCode, Is.EqualTo(
                    _dischargeParameter.MonitoringMethods.Single(m => m.MethodCode == ParametersAndMethodsConstants.MeanSectionMonitoringMethod).MethodCode));
            }
        }

        private static IEnumerable<DischargeActivity> GetAllResultDischargeActivities(IEnumerable<ParsedResult> results)
        {
            return results.Cast<NewFieldVisit>().SelectMany(r => r.FieldVisit.DischargeActivities);
        }

        [Test]
        public void ParseFile_ReturnsResultWithGageHeightValuesSetAccordingToGageHeightParameter()
        {
            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            foreach (var dischargeActivity in GetAllResultDischargeActivities(results))
            {
                Assert.That(dischargeActivity.GageHeightUnit, Is.EqualTo(_gageHeightParameter.DefaultUnit));
                Assert.That(dischargeActivity.GageHeightMethod.MethodCode, Is.EqualTo(
                    _gageHeightParameter.MonitoringMethods.Single(m => m.MethodCode == ParametersAndMethodsConstants.DefaultMonitoringMethod).MethodCode));
            }
        }

        [Test]
        public void ParseFile_SiteIdDoesNotMatchExistingLocation_Throws()
        {
            _parseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns((ILocationInfo)null);

            TestDelegate testDelegate = () => _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>());
        }

        [Test]
        public void ParseFile_SiteIdMatchesExistingLocation_RetrievesFieldVisitsForThatLocation()
        {
            _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            _locationInfo.Received()
                .FindLocationFieldVisitsInTimeRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
        }

        [Test]
        public void ParseFile_AppliesLocationOffsetToStartAndEndTimes()
        {
            var results = _pocketGaugerParser.ParseFile(_stream, _parseContext, _logger);

            var expectedOffset = TimeSpan.FromHours(_locationInfo.UtcOffsetHours);
            VerifyFieldVisitOffsets(results, expectedOffset);
            VerifyDischargeActivityOffsets(results, expectedOffset);
        }

        private static void VerifyFieldVisitOffsets(IEnumerable<ParsedResult> results, TimeSpan expectedOffset)
        {
            foreach (var newFieldVisit in results.Cast<NewFieldVisit>())
            {
                Assert.That(newFieldVisit.FieldVisit.StartDate.Offset, Is.EqualTo(expectedOffset));
                Assert.That(newFieldVisit.FieldVisit.EndDate.Offset, Is.EqualTo(expectedOffset));
            }
        }

        private static void VerifyDischargeActivityOffsets(ICollection<ParsedResult> results, TimeSpan expectedOffset)
        {
            foreach (var dischargeActivity in GetAllResultDischargeActivities(results))
            {
                Assert.That(dischargeActivity.StartTime.Offset, Is.EqualTo(expectedOffset));
                Assert.That(dischargeActivity.EndTime.Offset, Is.EqualTo(expectedOffset));
            }
        }
    }
}
