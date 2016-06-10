using System.Collections.Generic;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public class ExpectedPanelData
    {
        public static IReadOnlyCollection<PanelItem> CreateExpectedPanels()
        {
            return new List<PanelItem>
            {
                CreateFirstPanel(),
                CreateSecondPanel(),
                CreateThirdPanel(),
                CreateFourthPanel(),
                CreateFifthPanel(),
                CreateSixthPanel(),
                CreateSeventhPanel(),
                CreateEighthPanel(),
                CreateNinthPanel(),
                CreateTenthPanel(),
                CreateEleventhPanel(),
                CreateTwelfthPanel(),
                CreateThirteenthPanel(),
                CreateFourteenthPanel(),
                CreateFifteenthPanel()
            };
        }

        private static PanelItem CreateFirstPanel()
        {
            return new PanelItem
            {
                PanelId = "11",
                Stage = null,
                Area = 0,
                MeanVelocity = 0,
                Flow = 0,
                VerticalNumber = 1,
                Distance = 0,
                Depth = 0,
                SiteId = "DEMO4",
                SampleDepth = 0,
                GaugingId = "2",
                Verticals = CreateVerticalsForFirstPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForFirstPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 7,
                    VerticalNo = 1,
                    MeasurementNo = 1,
                    PanelId = "11",
                    Distance = 0,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO4",
                    GaugingId = "2"
                },
            };
        }

        private static PanelItem CreateSecondPanel()
        {
            return new PanelItem
            {
                PanelId = "12",
                Stage = null,
                Area = 1.25,
                MeanVelocity = 1,
                Flow = 1.25,
                VerticalNumber = 2,
                Distance = 2.5,
                Depth = 1,
                SiteId = "DEMO4",
                SampleDepth = 0.6,
                GaugingId = "2",
                Verticals = CreateVerticalsForSecondPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForSecondPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 8,
                    VerticalNo = 2,
                    MeasurementNo = 1,
                    PanelId = "12",
                    Distance = 2.5,
                    Depth = 1,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 100,
                    Velocity = 2,
                    Stage = null,
                    SiteId = "DEMO4",
                    GaugingId = "2"
                }
            };
        }

        private static PanelItem CreateThirdPanel()
        {
            return new PanelItem
            {
                PanelId = "13",
                Stage = null,
                Area = 3.75,
                MeanVelocity = 3,
                Flow = 11.25,
                VerticalNumber = 3,
                Distance = 5,
                Depth = 2,
                SiteId = "DEMO4",
                SampleDepth = 1.2,
                GaugingId = "2",
                Verticals = CreateVerticalsForThirdPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForThirdPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 9,
                    VerticalNo = 3,
                    MeasurementNo = 1,
                    PanelId = "13",
                    Distance = 5,
                    Depth = 2,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 200,
                    Velocity = 4,
                    Stage = null,
                    SiteId = "DEMO4",
                    GaugingId = "2"
                }
            };
        }

        private static PanelItem CreateFourthPanel()
        {
            return new PanelItem
            {
                PanelId = "14",
                Stage = null,
                Area = 3.75,
                MeanVelocity = 3,
                Flow = 11.25,
                VerticalNumber = 4,
                Distance = 7.5,
                Depth = 1,
                SiteId = "DEMO4",
                SampleDepth = 0.6,
                GaugingId = "2",
                Verticals = CreateVerticalsForFourthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForFourthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 10,
                    VerticalNo = 4,
                    MeasurementNo = 1,
                    PanelId = "14",
                    Distance = 7.5,
                    Depth = 1,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 100,
                    Velocity = 2,
                    Stage = null,
                    SiteId = "DEMO4",
                    GaugingId = "2"
                }
            };
        }

        private static PanelItem CreateFifthPanel()
        {
            return new PanelItem
            {
                PanelId = "15",
                Stage = null,
                Area = 1.25,
                MeanVelocity = 1,
                Flow = 1.25,
                VerticalNumber = 5,
                Distance = 10,
                Depth = 0,
                SiteId = "DEMO4",
                SampleDepth = 0,
                GaugingId = "2",
                Verticals = CreateVerticalsForFifthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForFifthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 11,
                    VerticalNo = 5,
                    MeasurementNo = 1,
                    PanelId = "15",
                    Distance = 10,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO4",
                    GaugingId = "2"
                }
            };
        }

        private static PanelItem CreateSixthPanel()
        {
            return new PanelItem
            {
                PanelId = "16",
                Stage = null,
                Area = 0,
                MeanVelocity = 0,
                Flow = 0,
                VerticalNumber = 1,
                Distance = 0,
                Depth = 0,
                SiteId = "DEMO5",
                SampleDepth = 0,
                GaugingId = "7",
                Verticals = CreateVerticalsForSixthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForSixthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 12,
                    VerticalNo = 1,
                    MeasurementNo = 1,
                    PanelId = "16",
                    Distance = 0,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO5",
                    GaugingId = "7"
                }
            };
        }

        private static PanelItem CreateSeventhPanel()
        {
            return new PanelItem
            {
                PanelId = "17",
                Stage = null,
                Area = 0.5,
                MeanVelocity = 1,
                Flow = 0.5,
                VerticalNumber = 2,
                Distance = 1,
                Depth = 1,
                SiteId = "DEMO5",
                SampleDepth = 0.6,
                GaugingId = "7",
                Verticals = CreateVerticalsForSeventhPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForSeventhPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 13,
                    VerticalNo = 2,
                    MeasurementNo = 1,
                    PanelId = "17",
                    Distance = 1,
                    Depth = 1,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 100,
                    Velocity = 2,
                    Stage = null,
                    SiteId = "DEMO5",
                    GaugingId = "7"
                }
            };
        }

        private static PanelItem CreateEighthPanel()
        {
            return new PanelItem
            {
                PanelId = "18",
                Stage = null,
                Area = 1.5,
                MeanVelocity = 3,
                Flow = 4.5,
                VerticalNumber = 3,
                Distance = 2,
                Depth = 2,
                SiteId = "DEMO5",
                SampleDepth = 1.2,
                GaugingId = "7",
                Verticals = CreateVerticalsForEighthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForEighthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 14,
                    VerticalNo = 3,
                    MeasurementNo = 1,
                    PanelId = "18",
                    Distance = 2,
                    Depth = 2,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 200,
                    Velocity = 4,
                    Stage = null,
                    SiteId = "DEMO5",
                    GaugingId = "7"
                }
            };
        }

        private static PanelItem CreateNinthPanel()
        {
            return new PanelItem
            {
                PanelId = "19",
                Stage = null,
                Area = 1,
                MeanVelocity = 2,
                Flow = 2,
                VerticalNumber = 4,
                Distance = 3,
                Depth = 0,
                SiteId = "DEMO5",
                SampleDepth = 0,
                GaugingId = "7",
                Verticals = CreateVerticalsForNinthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForNinthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 15,
                    VerticalNo = 4,
                    MeasurementNo = 1,
                    PanelId = "19",
                    Distance = 3,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO5",
                    GaugingId = "7"
                }
            };
        }

        private static PanelItem CreateTenthPanel()
        {
            return new PanelItem
            {
                PanelId = "20",
                Stage = null,
                Area = 0,
                MeanVelocity = 0,
                Flow = 0,
                VerticalNumber = 5,
                Distance = 4,
                Depth = 0,
                SiteId = "DEMO5",
                SampleDepth = 0,
                GaugingId = "7",
                Verticals = CreateVerticalsForTenthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForTenthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 16,
                    VerticalNo = 5,
                    MeasurementNo = 1,
                    PanelId = "20",
                    Distance = 4,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO5",
                    GaugingId = "7"
                }
            };
        }

        private static PanelItem CreateEleventhPanel()
        {
            return new PanelItem
            {
                PanelId = "26",
                Stage = null,
                Area = 0,
                MeanVelocity = 0,
                Flow = 0,
                VerticalNumber = 1,
                Distance = 0,
                Depth = 0,
                SiteId = "DEMO3",
                SampleDepth = 0,
                GaugingId = "1",
                Verticals = CreateVerticalsForEleventhPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForEleventhPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 2,
                    VerticalNo = 1,
                    MeasurementNo = 1,
                    PanelId = "26",
                    Distance = 0,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO3",
                    GaugingId = "1"
                }
            };
        }

        private static PanelItem CreateTwelfthPanel()
        {
            return new PanelItem
            {
                PanelId = "27",
                Stage = null,
                Area = 1.25,
                MeanVelocity = 1,
                Flow = 1.25,
                VerticalNumber = 2,
                Distance = 2.5,
                Depth = 1,
                SiteId = "DEMO3",
                SampleDepth = 0.6,
                GaugingId = "1",
                Verticals = CreateVerticalsForTwelfthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForTwelfthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 3,
                    VerticalNo = 2,
                    MeasurementNo = 1,
                    PanelId = "27",
                    Distance = 2.5,
                    Depth = 1,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 100,
                    Velocity = 2,
                    Stage = null,
                    SiteId = "DEMO3",
                    GaugingId = "1"
                }
            };
        }

        private static PanelItem CreateThirteenthPanel()
        {
            return new PanelItem
            {
                PanelId = "28",
                Stage = null,
                Area = 3.75,
                MeanVelocity = 3,
                Flow = 11.25,
                VerticalNumber = 3,
                Distance = 5,
                Depth = 2,
                SiteId = "DEMO3",
                SampleDepth = 1.2,
                GaugingId = "1",
                Verticals = CreateVerticalsForThirteenthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForThirteenthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 4,
                    VerticalNo = 3,
                    MeasurementNo = 1,
                    PanelId = "28",
                    Distance = 5,
                    Depth = 2,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 200,
                    Velocity = 4,
                    Stage = null,
                    SiteId = "DEMO3",
                    GaugingId = "1"
                }
            };
        }

        private static PanelItem CreateFourteenthPanel()
        {
            return new PanelItem
            {
                PanelId = "29",
                Stage = null,
                Area = 3.75,
                MeanVelocity = 3,
                Flow = 11.25,
                VerticalNumber = 4,
                Distance = 7.5,
                Depth = 1,
                SiteId = "DEMO3",
                SampleDepth = 0.6,
                GaugingId = "1",
                Verticals = CreateVerticalsForFourteenthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForFourteenthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 5,
                    VerticalNo = 4,
                    MeasurementNo = 1,
                    PanelId = "29",
                    Distance = 7.5,
                    Depth = 1,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 100,
                    Velocity = 2,
                    Stage = null,
                    SiteId = "DEMO3",
                    GaugingId = "1"
                }
            };
        }

        private static PanelItem CreateFifteenthPanel()
        {
            return new PanelItem
            {
                PanelId = "30",
                Stage = null,
                Area = 1.25,
                MeanVelocity = 1,
                Flow = 1.25,
                VerticalNumber = 5,
                Distance = 10,
                Depth = 0,
                SiteId = "DEMO3",
                SampleDepth = 0,
                GaugingId = "1",
                Verticals = CreateVerticalsForFifteenthPanel()
            };
        }

        private static IReadOnlyList<VerticalItem> CreateVerticalsForFifteenthPanel()
        {
            return new List<VerticalItem>
            {
                new VerticalItem
                {
                    VerticalId = 6,
                    VerticalNo = 5,
                    MeasurementNo = 1,
                    PanelId = "30",
                    Distance = 10,
                    Depth = 0,
                    SampleFromSurface = true,
                    MeterId = "12345±",
                    SamplePosition = 0.6,
                    ExposureTime = 50,
                    Revs = 0,
                    Velocity = 0,
                    Stage = null,
                    SiteId = "DEMO3",
                    GaugingId = "1"
                }
            };
        }
    }
}
