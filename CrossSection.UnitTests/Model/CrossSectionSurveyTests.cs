using System;
using NUnit.Framework;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.CrossSection.Model;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Model
{
    [TestFixture]
    public class CrossSectionSurveyTests
    {
        [Test]
        public void GetMetadataValue_NullMetadataKey_Throws()
        {
            var crossSection = new CrossSectionSurvey();

            TestDelegate testDelegate = () => crossSection.GetMetadataValue(null);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetMetadataValue_MetadataKeyDoesNotExist_Throws()
        {
            var crossSection = new CrossSectionSurvey();

            const string metadataKey = "SomeKey";

            TestDelegate testDelegate = () => crossSection.GetMetadataValue(metadataKey);

            Assert.That(testDelegate, Throws.Exception.TypeOf<ParsingFailedException>().With.Message.Contains(metadataKey));
        }

        [Test]
        public void GetMetadataValue_MetadataKeyExists_ReturnsExpectedValue()
        {
            var crossSection = new CrossSectionSurvey();

            const string metadataKey = "SomeKey";
            const string metadataValue = "value";
            crossSection.Metadata.Add(metadataKey, metadataValue);

            var result = crossSection.GetMetadataValue(metadataKey);

            Assert.That(result, Is.EqualTo(metadataValue));
        }
    }
}
