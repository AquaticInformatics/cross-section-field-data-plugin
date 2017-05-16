using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using MeterCalibration = Server.BusinessInterfaces.FieldDataPluginCore.DataModel.Meters.MeterCalibration;

namespace Server.Plugins.FieldVisit.PocketGauger.Interfaces
{
    public interface IMeterCalibrationMapper
    {
        MeterCalibration Map(MeterDetailsItem meterDetailsItem);
    }
}
