using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    [Serializable]
    internal class FactoryMethodNotInterfaceException : Exception
    {
        public FactoryMethodNotInterfaceException()
        {
        }

        public FactoryMethodNotInterfaceException(string message) : base(message)
        {
        }

        public FactoryMethodNotInterfaceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FactoryMethodNotInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}