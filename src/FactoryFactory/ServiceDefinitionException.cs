using System;
using System.Runtime.Serialization;

namespace FactoryFactory
{
    [Serializable]
    public class ServiceDefinitionException : Exception
    {
        public ServiceDefinitionException()
        {
        }

        protected ServiceDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceDefinitionException(string message) : base(message)
        {
        }

        public ServiceDefinitionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
