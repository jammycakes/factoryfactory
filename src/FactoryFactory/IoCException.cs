using System;
using System.Runtime.Serialization;

namespace FactoryFactory
{
    [Serializable]
    public class IoCException : Exception
    {
        public IoCException()
        {
        }

        protected IoCException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IoCException(string message) : base(message)
        {
        }

        public IoCException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
