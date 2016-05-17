using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Server.BusinessInterfaces.FieldDataPlugInCore;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.BusinessInterfaces.FieldDataPlugInCore.Results;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerParser : IFieldDataPlugIn
    {
        public ICollection<ParsedResult> ParseFile(Stream fileStream, IParseContext context, ILog logger)
        {
            using (var zipFile = GetZipFile(fileStream))
            using (var zipContents = GetZipContents(zipFile))
            {
                zipFile.IsStreamOwner = false;
                if (!zipContents.ContainsKey(FileNames.GaugingSummary))
                {
                    throw new FormatNotSupportedException(
                        Invariant($"Zip file does not contain file {FileNames.GaugingSummary}"));
                }
            }

            return new List<ParsedResult>();
        }

        private static ZipFile GetZipFile(Stream fileStream)
        {
            try
            {
                return new ZipFile(fileStream);
            }
            catch (ZipException)
            {
                throw new FormatNotSupportedException("fileStream is not a zip file");
            }
        }

        private static PocketGaugerFiles GetZipContents(ZipFile zipFile)
        {
            var streams = new PocketGaugerFiles();
            foreach (ZipEntry zipEntry in zipFile)
            {
                streams.Add(zipEntry.Name.ToLower(), zipFile.GetInputStream(zipEntry));
            }

            return streams;
        }
    }
}
