using System;
using System.Runtime.Serialization;

namespace Microsoft.Extensions.DependencyInjection.AddFactoryExtension
{
    [Serializable]
    internal class ConflictingCtorsFoundException : Exception
    {
        public ConflictingCtorsFoundException()
        {
        }

        public ConflictingCtorsFoundException(string message) : base(message)
        {
        }

        public ConflictingCtorsFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConflictingCtorsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}