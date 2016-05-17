using System;
using System.Collections.Generic;
using System.IO;

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
    }
}
