using System;
using System.Linq;
using System.Reflection;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    public class PrivateConstructorHelper
    {
        public static T CreateInstance<T>(params object[] parameters) where T : class
        {
            var classType = typeof(T);
            var parameterTypes = parameters
                .Select(p => p.GetType())
                .ToArray();

            var constructor = classType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);

            if (constructor == null)
                throw new Exception($"Can't find private {classType.FullName}({string.Join(", ", parameterTypes.Select(p => p.FullName))}) constructor");

            var instance = (T)constructor.Invoke(parameters);

            return instance;
        }
    }
}
