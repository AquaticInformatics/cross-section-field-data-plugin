using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Exceptions
{
    [Serializable]
    public class PocketGaugerZipFileException : Exception
    {
        protected PocketGaugerZipFileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PocketGaugerZipFileException()
        {
        }

        public PocketGaugerZipFileException(string message)
            : base(message)
        {
        }

        public PocketGaugerZipFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}