
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    public interface ITestRecord<out TRecord> where TRecord : ISelfValidator
    {
        TRecord AParametricRecord(int ordinal);
    }
}
