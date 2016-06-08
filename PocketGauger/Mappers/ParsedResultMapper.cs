using System;
using System.Collections.Generic;
using System.Linq;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.DataModel.DischargeActivities;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Interfaces;

namespace Server.Plugins.FieldVisit.PocketGauger.Mappers
{
    public class ParsedResultMapper
    {
        private readonly IParseContext _context;
        private readonly IDischargeActivityMapper _dischargeActivityMapper;

        public ParsedResultMapper(IParseContext context, IDischargeActivityMapper dischargeActivityMapper)
        {
            _context = context;
            _dischargeActivityMapper = dischargeActivityMapper;
        }

        public List<ParsedResult> CreateParsedResults(GaugingSummary gaugingSummary)
        {
            var parsedResults = new List<ParsedResult>();
            foreach (var gaugingSummaryItem in gaugingSummary.GaugingSummaryItems)
            {
                var locationInfo = GetLocationInfoOrThrow(gaugingSummaryItem.SiteId);

                var dischargeActivity = _dischargeActivityMapper.Map(locationInfo, gaugingSummaryItem);

                parsedResults.Add(CreateParsedResult(locationInfo, dischargeActivity));
            }

            return parsedResults;
        }

        private ILocationInfo GetLocationInfoOrThrow(string locationIdentifier)
        {
            var locationInfo = _context.FindLocationByIdentifier(locationIdentifier);

            if (locationInfo == null)
            {
                throw new ParsingFailedException(
                    FormattableString.Invariant($"Location with identifier: {locationIdentifier} does not exist"));
            }

            return locationInfo;
        }

        private static ParsedResult CreateParsedResult(ILocationInfo locationInfo, DischargeActivity dischargeActivity)
        {
            var existingFieldVisits = locationInfo.FindLocationFieldVisitsInTimeRange(dischargeActivity.StartTime, dischargeActivity.EndTime);

            if (existingFieldVisits.Any())
            {
                return CreateNewDischargeActivityParsedResult(dischargeActivity, existingFieldVisits.First());
            }

            return CreateNewFieldVisit(locationInfo, dischargeActivity);
        }

        private static NewDischargeActivity CreateNewDischargeActivityParsedResult(DischargeActivity dischargeActivity,
            IFieldVisitInfo fieldVisitInfo)
        {
            return new NewDischargeActivity
            {
                DischargeActivity = dischargeActivity,
                FieldVisit = fieldVisitInfo
            };
        }

        private static NewFieldVisit CreateNewFieldVisit(ILocationInfo locationInfo, DischargeActivity dischargeActivity)
        {
            return new NewFieldVisit
            {
                Location = locationInfo,
                FieldVisit = new BusinessInterfaces.FieldDataPlugInCore.DataModel.FieldVisit
                {
                    StartDate = dischargeActivity.StartTime,
                    EndDate = dischargeActivity.EndTime,
                    Party = dischargeActivity.Party,
                    DischargeActivities = new[] { dischargeActivity }
                }
            };
        }
    }
}
