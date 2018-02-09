using System;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Model;
using NUnit.Framework;

namespace CrossSectionPlugin.UnitTests.Model
{
    [TestFixture]
    public class CrossSectionSurveyTests
    {
        [Test]
        public void GetFieldValue_NullField_Throws()
        {
            var crossSection = new CrossSectionSurvey();

            TestDelegate testDelegate = () => crossSection.GetFieldValue(null);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetFieldValue_FieldDoesNotExist_Throws()
        {
            var crossSection = new CrossSectionSurvey();

            const string field = "SomeKey";

            TestDelegate testDelegate = () => crossSection.GetFieldValue(field);

            Assert.That(testDelegate, Throws.Exception.TypeOf<CrossSectionCsvFormatException>().With.Message.Contains(field));
        }

        [Test]
        public void GetFieldValue_FieldExists_ReturnsExpectedValue()
        {
            var crossSection = new CrossSectionSurvey();

            const string field = "SomeKey";
            const string data = "value";
            crossSection.Fields.Add(field, data);

            var result = crossSection.GetFieldValue(field);

            Assert.That(result, Is.EqualTo(data));
        }
    }
}
