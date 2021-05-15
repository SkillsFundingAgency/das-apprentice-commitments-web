using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    public class InvalidHashedIdException : Exception
    {
        public InvalidHashedIdException(string? hashValue)
            : base($"Invalid hashed ID value '{hashValue}'")
        {
        }
    }
}