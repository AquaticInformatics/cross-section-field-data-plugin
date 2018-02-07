using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.CrossSection.Exceptions
{
    [Serializable]
    public class CrossSectionSurveyDataFormatException : Exception
    {
        public CrossSectionSurveyDataFormatException()
        {
        }

        public CrossSectionSurveyDataFormatException(string message) : base(message)
        {
        }

        public CrossSectionSurveyDataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CrossSectionSurveyDataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}