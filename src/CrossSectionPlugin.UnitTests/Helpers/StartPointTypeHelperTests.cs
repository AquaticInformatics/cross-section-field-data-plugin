﻿using System;
using System.Collections.Generic;
using CrossSectionPlugin.Exceptions;
using CrossSectionPlugin.Helpers;
using FieldDataPluginFramework.DataModel.ChannelMeasurements;
using NUnit.Framework;

namespace CrossSectionPlugin.UnitTests.Helpers
{
    [TestFixture]
    public class StartPointTypeHelperTests
    {
        [TestCaseSource(nameof(InvalidStartPointTypeStrings))]
        public void Parse_ValueIsNotValid_Throws(string invalidOrUnknownStartPointTypeStrings)
        {
            TestDelegate testDelegate = () => StartPointTypeHelper.Parse(invalidOrUnknownStartPointTypeStrings);

            Assert.That(testDelegate, Throws.TypeOf<InvalidStartBankException>());
        }

        private static readonly List<string> InvalidStartPointTypeStrings = new List<string>
        {
            "L",
            "R",
            Guid.NewGuid().ToString(),
            "",
            null,
            "About the LeftBank",
            "Unspecified"
        };

        [TestCaseSource(nameof(LeftEdgeOfWaterTestCases))]
        public void Parse_ValueIsExpectedLeftEdgeOfWater_ReturnsLeftEdgeOfWater(string leftEdgeOfWaterString)
        {
            var result = StartPointTypeHelper.Parse(leftEdgeOfWaterString);

            Assert.That(result, Is.EqualTo(StartPointType.LeftEdgeOfWater));
        }

        private static readonly List<string> LeftEdgeOfWaterTestCases = new List<string>
        {
            "LeftEdgeOfWater",
            "Left Edge Of Water",
            "Left edge OF WATER",
            "LEfTEdGEOFWaTER",
            "Left",
            "LeftBank",
            "LEW"
        };

        [TestCaseSource(nameof(RightEdgeOfWaterTestCases))]
        public void Parse_ValueIsExpectedRightEdgeOfWater_ReturnsRightEdgeOfWater(string rightEdgeOfWaterString)
        {
            var result = StartPointTypeHelper.Parse(rightEdgeOfWaterString);

            Assert.That(result, Is.EqualTo(StartPointType.RightEdgeOfWater));
        }

        private static readonly List<string> RightEdgeOfWaterTestCases = new List<string>
        {
            "RightEdgeOfWater",
            "Right Edge Of Water",
            "RIGHT edge OF WATER",
            "RightEdGEOFWaTER",
            "Right",
            "RightBank",
            "REW"
        };
    }
}
