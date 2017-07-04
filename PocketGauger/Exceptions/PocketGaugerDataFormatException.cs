using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.PocketGauger.Exceptions
{
    [Serializable]
    public class PocketGaugerDataFormatException : Exception
    {
        public PocketGaugerDataFormatException()
        {
        }

        public PocketGaugerDataFormatException(string message) : base(message)
        {
        }

        public PocketGaugerDataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PocketGaugerDataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}