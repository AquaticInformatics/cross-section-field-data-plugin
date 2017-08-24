using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.StageDischarge.Exceptions
{
    [Serializable]
    public class StageDischargeDataFormatException : Exception
    {
        public StageDischargeDataFormatException()
        {
        }

        public StageDischargeDataFormatException(Exception e) : base(e.Message, e)
        {
        }

        public StageDischargeDataFormatException(string message) : base(message)
        {
        }

        public StageDischargeDataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StageDischargeDataFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}