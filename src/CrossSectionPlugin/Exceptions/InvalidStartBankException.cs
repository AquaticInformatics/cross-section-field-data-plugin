using System;
using System.Runtime.Serialization;

namespace CrossSectionPlugin.Exceptions
{
    [Serializable]
    public class InvalidStartBankException : Exception
    {
        public InvalidStartBankException()
        {
        }

        public InvalidStartBankException(string message) : base(message)
        {
        }

        public InvalidStartBankException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidStartBankException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}