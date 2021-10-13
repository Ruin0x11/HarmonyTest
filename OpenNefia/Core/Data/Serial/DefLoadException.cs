using System;
using System.Runtime.Serialization;

namespace OpenNefia.Core.Data.Serial
{
    [Serializable]
    internal class DefLoadException : Exception
    {
        public DefLoadException()
        {
        }

        public DefLoadException(string? message) : base(message)
        {
        }

        public DefLoadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DefLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}