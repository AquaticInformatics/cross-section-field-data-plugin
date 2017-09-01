using System;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;
using Server.Plugins.FieldVisit.StageDischarge.UnitTests.TestData;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests
{
    [TestFixture]
    internal class StageDischargeRecordTests
    {
        private IFixture _fixture;

        [SetUp]
        public void BeforeTests()
        {
            _fixture = new Fixture();
        }

        [TestCase("LocationIdentifier")]
        [TestCase("StageAtStart")]
        [TestCase("StageAtEnd")]
        [TestCase("StageUnits")]
        [TestCase("Discharge")]
        [TestCase("DischargeUnits")]
        [TestCase("ChannelName")]
        [TestCase("ChannelWidth")]
        [TestCase("WidthUnits")]
        [TestCase("AreaUnits")]
        [TestCase("VelocityUnits")]
        public void StageDischargeRecord_SelfValidateWithNullPropertyValue_DetectsNull(string propertyName)
        {
            var stageDischargeRecord = StageDischargeCsvFileBuilder.CreateFullRecord(_fixture);
            CheckExpectedExceptionAndMessageWhenSpecifiedFieldIsNull<ArgumentNullException>(stageDischargeRecord, propertyName);
        }

        private void CheckExpectedExceptionAndMessageWhenSpecifiedFieldIsNull<E>(StageDischargeRecord stageDischargeRecord, string propertyName) where E : Exception
        {
            var field = stageDischargeRecord.GetType().GetField(propertyName);
            if (field != null)
            {
                field.SetValue(stageDischargeRecord, null);
            }

            Action validationAction = () => stageDischargeRecord.Validate();
            validationAction
                .ShouldThrow<E>()
                .And.Message.Should().Contain(propertyName);
        }

        [Test]
        public void StageDischargeRecord_SelfValidateWithAllRequiredValues_DoesNotThrow()
        {
            var stageDischargeRecord = StageDischargeCsvFileBuilder.CreateFullRecord(_fixture);
            Action noThrowAction = () => stageDischargeRecord.Validate();
            noThrowAction.ShouldNotThrow();
        }

        [Test]
        public void StageDischargeRecord_SelfValidate_Timestamps()
        {
            StageDischargeRecord stageDischargeRecord = StageDischargeCsvFileBuilder.CreateFullRecord(_fixture);
            Action validationAction = () => stageDischargeRecord.Validate();
            stageDischargeRecord.MeasurementStartDateTime = DateTimeOffset.Now;
            stageDischargeRecord.MeasurementEndDateTime = stageDischargeRecord.MeasurementStartDateTime;
            validationAction.ShouldNotThrow();

            stageDischargeRecord.MeasurementEndDateTime = DateTimeOffset.Now.AddDays(1);
            validationAction.ShouldNotThrow();

            stageDischargeRecord.MeasurementStartDateTime = DateTimeOffset.Now.AddDays(200);
            validationAction.ShouldThrow<ArgumentException>().And.Message.Should().Contain("MeasurementStartDateTime");
        }
    }
}
