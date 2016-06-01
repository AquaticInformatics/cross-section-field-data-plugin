using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class ExtensionMethods
    {
        public static IUnit GetParameterDefaultUnit(this IParseContext context, string parameterId)
        {
            return context.AllParameters.First(parameter => parameter.Id == parameterId).DefaultUnit;
        }

        public static IMonitoringMethod GetDefaultMonitoringMethod(this IParseContext context)
        {
            return GetMonitoringMethod(context.AllParameters.First(), ParametersAndMethodsConstants.DefaultMonitoringMethod);
        }

        public static IMonitoringMethod GetMonitoringMethod(this IParameter parameter, string monitoringMethod)
        {
            return parameter.MonitoringMethods.First(method => method.MethodCode == monitoringMethod);
        }
    }
}
