
namespace Server.Plugins.FieldVisit.StageDischarge.Parsers
{
    public interface ITestRecord<out TRecord> where TRecord : ISelfValidator
    {
        TRecord AParametricRecord(int ordinal);
    }
}
