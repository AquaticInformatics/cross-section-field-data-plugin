using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using Server.Plugins.FieldVisit.PocketGauger.Mappers;
using Server.Plugins.FieldVisit.PocketGauger.Parsers;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser : IFieldDataPlugIn
    {
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            using (var zipArchive = GetZipArchive(fileStream))
            using (var zipContents = GetZipContents(zipArchive))
            {
                if (!zipContents.ContainsKey(FileNames.GaugingSummary))
                {
                    throw new FormatNotSupportedException(
                        FormattableString.Invariant($"Zip file does not contain file {FileNames.GaugingSummary}"));
                }

                var gaugingSummary = CreateGaugingSummaryAssembler().Assemble(zipContents);

                return CreateParsedResults(context, gaugingSummary);
            }
        }

        private static ZipArchive GetZipArchive(Stream fileStream)
        {
            try
            {
                return new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen:true);
            }
            catch (InvalidDataException)
            {
                throw new FormatNotSupportedException("fileStream is not a zip file");
            }
        }

        private static PocketGaugerFiles GetZipContents(ZipArchive zipArchive)
        {
            var streams = new PocketGaugerFiles();
            foreach (var zipArchiveEntry in zipArchive.Entries)
            {
                streams.Add(zipArchiveEntry.Name.ToLower(), zipArchiveEntry.Open());
            }

            return streams;
        }

        private static GaugingSummaryAssembler CreateGaugingSummaryAssembler()
        {
            return new GaugingSummaryAssembler(new GaugingSummaryParser(), new MeterDetailsParser(), new PanelParser());
        }

        public List<ParsedResult> CreateParsedResults(IParseContext context, GaugingSummary gaugingSummary)
        {
            var meterCalibrationMapper = new MeterCalibrationMapper(context);
            var verticalMapper = new VerticalMapper(meterCalibrationMapper);
            var pointVelocityMapper = new PointVelocityMapper(context);
            var dischargeActivityMapper = new DischargeActivityMapper(context, verticalMapper, pointVelocityMapper);
            var parsedResultMapper = new ParsedResultMapper(context, dischargeActivityMapper);

            return parsedResultMapper.CreateParsedResults(gaugingSummary);
       }
    }
}
