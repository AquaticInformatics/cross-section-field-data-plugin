using System;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    class StageDischargeRecordTests
    {
        private IFixture _fixture;

        [SetUp]
        public void BeforeTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void StageDischargeRecord_SelfValidate_detectsNullsInRequiredFields()
        {
            StageDischargeRecord stageDischargeRecord = StageDischargeRecordBuilder.Build().ARecord();
            Action validationAction = () => stageDischargeRecord.Validate();

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "LocationIdentifier");
            stageDischargeRecord.LocationIdentifier = "";
            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "LocationIdentifier");
            stageDischargeRecord.LocationIdentifier = _fixture.Create<string>();

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "StageAtStart");
            stageDischargeRecord.StageAtStart = _fixture.Create<double>();

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "StageAtEnd");
            stageDischargeRecord.StageAtEnd = _fixture.Create<double>(); ;

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "StageUnits");
            stageDischargeRecord.StageUnits = "m/s";

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "Discharge");
            stageDischargeRecord.Discharge = _fixture.Create<double>(); ;

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "DischargeUnits");
            stageDischargeRecord.DischargeUnits = "m^3/s";

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "ChannelName");
            stageDischargeRecord.ChannelName = _fixture.Create<string>();

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "ChannelWidth");
            stageDischargeRecord.ChannelWidth = _fixture.Create<double>(); ;

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "WidthUnits");
            stageDischargeRecord.WidthUnits = "m";

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "AreaUnits");
            stageDischargeRecord.AreaUnits = "m^2";

            CheckExpectedExceptionAndMessage<ArgumentNullException>(validationAction, "VelocityUnits");
            stageDischargeRecord.VelocityUnits = "m/s";

            validationAction.ShouldNotThrow();
        }

        private void CheckExpectedExceptionAndMessage<E>(Action validate, string messagePart) where E : Exception
        {
            validate
                .ShouldThrow<E>()
                .And.Message.Should().Contain(messagePart);
        }

        [Test]
        public void StageDischargeRecord_SelfValidate_Timestamps()
        {
            StageDischargeRecord stageDischargeRecord = StageDischargeCsvFileBuilder.CreateFullRecord(_fixture);
            Action validationAction = () => stageDischargeRecord.Validate();
            stageDischargeRecord.MeasurementStartDateTime = DateTime.Now;
            stageDischargeRecord.MeasurementEndDateTime = stageDischargeRecord.MeasurementStartDateTime;
            validationAction.ShouldNotThrow();

            stageDischargeRecord.MeasurementEndDateTime = DateTime.Now.AddDays(1);
            validationAction.ShouldNotThrow();

            stageDischargeRecord.MeasurementStartDateTime = DateTime.Now.AddDays(200);
            CheckExpectedExceptionAndMessage<ArgumentException>(validationAction, "MeasurementStartDateTime");
        }
    }
}
