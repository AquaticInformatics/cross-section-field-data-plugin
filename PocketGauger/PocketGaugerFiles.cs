using System;
using System.Collections.Generic;
using System.IO;
using Server.BusinessInterfaces.FieldDataPlugInCore.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;
using static System.FormattableString;

namespace Server.Plugins.FieldVisit.PocketGauger
{
    public class PocketGaugerFiles : Dictionary<string, Stream>, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            
            foreach (var item in Values)
            {
                item.Dispose();
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public TPocketGaugerDto ParseType<TPocketGaugerDto>()
        {
            var pocketGaugerFileType = typeof(TPocketGaugerDto);

            if (!FileNames.FileNameTypeMap.ContainsKey(pocketGaugerFileType))
                throw new ArgumentException(Invariant($"Unknown type { pocketGaugerFileType.Name }"));

            return DeserializeFile<TPocketGaugerDto>(FileNames.FileNameTypeMap[pocketGaugerFileType]);
        }

        private TPocketGaugerDto DeserializeFile<TPocketGaugerDto>(string fileName)
        {
            if (!ContainsKey(fileName))
                throw new ParsingFailedException(
                    Invariant($"Zip file does not contain file {fileName}"));

            return XmlDeserializerHelper.Deserialize<TPocketGaugerDto>(fileName, this[fileName]);
        }
    }
}
