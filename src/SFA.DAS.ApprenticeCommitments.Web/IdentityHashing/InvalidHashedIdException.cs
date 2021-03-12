using System;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing
{
    public class InvalidHashedIdException : Exception
    {
        public InvalidHashedIdException()
        {
        }

        public InvalidHashedIdException(string? hashValue)
            : base($"Invalid hashed ID value '{hashValue}'")
        {
        }

        public InvalidHashedIdException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}