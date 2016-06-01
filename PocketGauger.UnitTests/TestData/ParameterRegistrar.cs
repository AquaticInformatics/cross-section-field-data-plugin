using System.Linq;
using NSubstitute;
using Ploeh.AutoFixture;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public class ParameterRegistrar
    {
        public static void Register(IFixture fixture)
        {
            fixture.Register(() => CreateUnit(fixture));
            fixture.Register(() => CreateMonitoringMethod(fixture));
            fixture.Register(() => CreateParameter(fixture));
        }

        private static IUnit CreateUnit(IFixture fixture)
        {
            var unit = Substitute.For<IUnit>();
            unit.UnitId.ReturnsForAnyArgs(fixture.Create<string>());
            unit.Name.ReturnsForAnyArgs(fixture.Create<string>());
            
            return unit;
        }

        private static IMonitoringMethod CreateMonitoringMethod(IFixture fixture)
        {
            var monitoringMethod = Substitute.For<IMonitoringMethod>();
            monitoringMethod.MethodCode.ReturnsForAnyArgs(fixture.Create<string>());
            monitoringMethod.Description.ReturnsForAnyArgs(fixture.Create<string>());
            monitoringMethod.DisplayName.ReturnsForAnyArgs(fixture.Create<string>());

            return monitoringMethod;
        }

        private static IParameter CreateParameter(IFixture fixture)
        {
            var parameter = Substitute.For<IParameter>();
            parameter.Id.ReturnsForAnyArgs(fixture.Create<string>());
            parameter.DisplayId.ReturnsForAnyArgs(fixture.Create<string>());
            parameter.DisplayName.ReturnsForAnyArgs(fixture.Create<string>());

            parameter.DefaultUnit.ReturnsForAnyArgs(fixture.Create<IUnit>());
            parameter.Units.ReturnsForAnyArgs(fixture.CreateMany<IUnit>().ToList());
            parameter.MonitoringMethods.ReturnsForAnyArgs(fixture.CreateMany<IMonitoringMethod>().ToList());

            return parameter;
        }
    }
}
