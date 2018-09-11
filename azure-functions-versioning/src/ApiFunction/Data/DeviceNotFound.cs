using System;
using System.Runtime.Serialization;

namespace ApiFunction.Data
{
    [Serializable]
    internal class DeviceNotFound : Exception
    {
        public DeviceNotFound()
        {
        }

        public DeviceNotFound(string message) : base(message)
        {
        }

        public DeviceNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DeviceNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}