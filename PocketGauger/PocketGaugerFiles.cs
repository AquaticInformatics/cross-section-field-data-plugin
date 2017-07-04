using System;
using System.Collections.Generic;
using System.IO;
using Server.Plugins.FieldVisit.PocketGauger.Exceptions;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

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
                throw new ArgumentException(string.Format("Unknown type {0}", pocketGaugerFileType.Name));

            return DeserializeFile<TPocketGaugerDto>(FileNames.FileNameTypeMap[pocketGaugerFileType]);
        }

        private TPocketGaugerDto DeserializeFile<TPocketGaugerDto>(string fileName)
        {
            if (!ContainsKey(fileName))
                throw new PocketGaugerZipFileMissingRequiredContentException(
                    string.Format("Zip file does not contain file {0}", fileName));

            return XmlDeserializerHelper.Deserialize<TPocketGaugerDto>(this[fileName]);
        }
    }
}
