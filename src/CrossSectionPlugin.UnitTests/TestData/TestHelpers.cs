﻿using System.Collections.Generic;

namespace CrossSectionPlugin.UnitTests.TestData
{
    public class TestHelpers
    {
        public static IDictionary<string, string> CreateExpectedCrossSectionFields()
        {
            return new Dictionary<string, string>
            {
                { "Location", "Server.Plugins.FieldVisit.CrossSection.Tests.CompleteCrossSection" },
                { "StartDate", "2001-05-08T14:32:15+07:00" },
                { "EndDate", "2001-05-08T17:12:45+07:00" },
                { "Party", "Cross-Section Party" },
                { "Channel", "Right overflow" },
                { "RelativeLocation", "At the Gage" },
                { "Stage", "12.2" },
                { "Unit", "ft" },
                { "StartBank", "Left bank" },
                { "Comment", "Cross-section survey comments" }
            };
        }
    }
}
