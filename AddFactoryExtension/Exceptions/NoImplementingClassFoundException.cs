using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    [Serializable]
    internal class NoImplementingClassFoundException : Exception
    {
        public NoImplementingClassFoundException()
        {
        }

        public NoImplementingClassFoundException(string message) : base(message)
        {
        }

        public NoImplementingClassFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoImplementingClassFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}