using System;
using System.IO;
using System.Xml.Serialization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.Dtos;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public static class GaugingSummaryParser
    {
        public static GaugingSummary Parse(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(GaugingSummary));

            try
            {
                return (GaugingSummary)serializer.Deserialize(fileStream);
            }
            catch (InvalidOperationException ex)
            {
                throw new ParsingFailedException(Invariant($"Parsing {FileNames.GaugingSummary} failed"), ex);
            }
        }
    }
}
