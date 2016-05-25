using System;
using System.IO;
using System.Xml.Serialization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class XmlDeserializerHelper
    {
        public static TPocketGaugerDto Deserialize<TPocketGaugerDto>(string fileName, Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(TPocketGaugerDto));

            try
            {
                return (TPocketGaugerDto)serializer.Deserialize(fileStream);
            }
            catch (InvalidOperationException ex)
            {
                throw new ParsingFailedException(Invariant($"Parsing {fileName} failed"), ex);
            }
        }
    }
}
