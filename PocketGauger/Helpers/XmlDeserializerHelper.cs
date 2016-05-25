using System;
using System.IO;
using System.Xml.Serialization;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger.Helpers
{
    public static class XmlDeserializerHelper
    {
        public static TPocketGaugerDto Deserialize<TPocketGaugerDto>(Stream fileStream)
        {
            var pocketGaugerType = typeof (TPocketGaugerDto);
            var serializer = new XmlSerializer(pocketGaugerType);

            try
            {
                return (TPocketGaugerDto)serializer.Deserialize(fileStream);
            }
            catch (InvalidOperationException ex)
            {
                throw new ParsingFailedException(Invariant($"Parsing {FileNames.FileNameTypeMap[pocketGaugerType]} failed"), ex);
            }
        }
    }
}
