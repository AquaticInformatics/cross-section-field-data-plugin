﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.CrossSection.Helpers;

namespace Server.Plugins.FieldVisit.CrossSection.UnitTests.Helpers
{
    [TestFixture]
    public class DoubleHelperTests
    {
        private IFixture _fixture;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void Parse_NullString_ReturnsNull()
        {
            var result = DoubleHelper.Parse(null);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Parse_NonDoubleString_ReturnsNull()
        {
            var result = DoubleHelper.Parse(_fixture.Create<string>());

            Assert.That(result, Is.Null);
        }

        private readonly List<Tuple<string, double>> _doubleValueTestCases = new List<Tuple<string, double>>
        {
            Tuple.Create("1234", 1234d),
            Tuple.Create("1234.4321", 1234.4321),
            Tuple.Create("0", 0d)
        };

        [TestCaseSource(nameof(_doubleValueTestCases))]
        public void Parse_ValidDoubleString_ReturnsExpectedValue(Tuple<string, double> testCases)
        {
            var value = testCases.Item1;
            var expected = testCases.Item2;

            var result = DoubleHelper.Parse(value);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
