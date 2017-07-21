using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Exceptions
{
    [Serializable]
    public class PocketGaugerDataPersistanceException : Exception
    {
        public PocketGaugerDataPersistanceException()
        {
        }

        public PocketGaugerDataPersistanceException(string message) : base(message)
        {
        }

        public PocketGaugerDataPersistanceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PocketGaugerDataPersistanceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
