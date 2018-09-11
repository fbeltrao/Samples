using System;
using System.Runtime.Serialization;

namespace ApiFunction.Data
{
    [Serializable]
    internal class DuplicateDeviceException : Exception
    {
        public DuplicateDeviceException()
        {
        }

        public DuplicateDeviceException(string message) : base(message)
        {
        }

        public DuplicateDeviceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateDeviceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}