using System;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public class ProxyTypeSpecimenBuilder : ISpecimenBuilder
    {
        private const string ProxySuffix = "Proxy";

        public object Create(object request, ISpecimenContext context)
        {
            var propertyInfo = request as PropertyInfo;
            if (propertyInfo == null)
            {
                return new NoSpecimen(request);
            }
            
            if (propertyInfo.PropertyType != typeof(string) || !propertyInfo.Name.EndsWith(ProxySuffix))
            {
                return new NoSpecimen(request);
            }

            var nonProxyProperty = GetNonProxyProperty(propertyInfo);

            if (nonProxyProperty == null)
                return new NoSpecimen(request);

            var value = context.Resolve(nonProxyProperty.PropertyType);

            return FormatValueAsExpectedString(value);
        }

        private static PropertyInfo GetNonProxyProperty(PropertyInfo propertyInfo)
        {
            var nonProxyName = propertyInfo.Name.Replace(ProxySuffix, string.Empty);

            return propertyInfo.DeclaringType?.GetProperty(nonProxyName);
        }

        private static object FormatValueAsExpectedString(object value)
        {
            var valueType = value.GetType();

            if (valueType == typeof(DateTime))
                return DateTimeHelper.Serialize((DateTime)value);

            if (valueType == typeof(bool))
                return BooleanHelper.Serialize((bool)value);

            if (valueType == typeof(double?))
                return DoubleHelper.Serialize((double?)value);

            if (valueType == typeof(int?))
                return IntHelper.Serialize((int?)value);

            return value.ToString();
        }
    }
}
