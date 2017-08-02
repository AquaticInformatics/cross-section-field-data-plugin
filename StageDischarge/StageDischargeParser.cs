using System;
using System.IO;
using Server.BusinessInterfaces.FieldDataPluginCore;
using Server.BusinessInterfaces.FieldDataPluginCore.Context;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel;
using Server.BusinessInterfaces.FieldDataPluginCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPluginCore.Results;

namespace Server.Plugins.FieldVisit.StageDischarge
{
    public class StageDischargeParser : IFieldDataPlugin
    {
        public ParseFileResult ParseFile(Stream fileStream, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            try
            {
                return ParseFileResult.ParsedSuccessfully();
            }
            catch (Exception e)
            {
                return ParseFileResult.ParsingFailed(e);
            }
        }

        public ParseFileResult ParseFile(Stream fileStream, LocationInfo selectedLocation, IFieldDataResultsAppender fieldDataResultsAppender,
            ILog logger)
        {
            return ParseFile(fileStream, fieldDataResultsAppender, logger);
        }

        private static FieldVisitDetails CreateFieldVisit(DischargeActivity dischargeActivity)
        {
            return new FieldVisitDetails(dischargeActivity.MeasurementPeriod)
            {
                Party = dischargeActivity.Party
            };
        }
    }
}
