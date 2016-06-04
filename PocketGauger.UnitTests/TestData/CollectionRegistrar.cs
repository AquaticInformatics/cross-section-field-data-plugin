using System.Collections.Generic;
using Ploeh.AutoFixture;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public static class CollectionRegistrar
    {
        public static void Register(IFixture fixture)
        {
            fixture.Register<IReadOnlyList<MeterCalibrationItem>>(fixture.Create<List<MeterCalibrationItem>>);
            fixture.Register<IReadOnlyCollection<PanelItem>>(fixture.Create<List<PanelItem>>);
            fixture.Register<IReadOnlyList<VerticalItem>>(fixture.Create<List<VerticalItem>>);
        }
    }
}
