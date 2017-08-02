using System;
using System.Runtime.Serialization;

namespace Server.Plugins.FieldVisit.StageDischarge.Exceptions
{
    [Serializable]
    public class StageDischargeDataPersistenceException : Exception
    {
        public StageDischargeDataPersistenceException()
        {
        }

        public StageDischargeDataPersistenceException(string message) : base(message)
        {
        }

        public StageDischargeDataPersistenceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StageDischargeDataPersistenceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
