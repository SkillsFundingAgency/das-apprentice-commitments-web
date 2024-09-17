using System;
using System.Runtime.Serialization;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    [Serializable]
    public class InvalidHashedIdException : Exception
    {
        public InvalidHashedIdException(string? hashValue)
            : base($"Invalid hashed ID value '{hashValue}'")
        {
        }

        protected InvalidHashedIdException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}