using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.CrossSection.Exceptions
{
    [Serializable]
    public class CrossSectionCsvFormatException : Exception
    {
        public CrossSectionCsvFormatException()
        {
        }

        public CrossSectionCsvFormatException(string message) : base(message)
        {
        }

        public CrossSectionCsvFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CrossSectionCsvFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}