using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class HashedId
    {
        public string Hashed { get; }

        public HashedId(string hashed, string allowedCharacters = "abcdefgh12345678")
        {
            if (!hashed.All(c => allowedCharacters.Contains(c)))
                throw new InvalidHashedIdException(hashed);
            Hashed = hashed;
        }

        public override bool Equals(object obj) => obj switch
        {
            HashedId other => other.Hashed == Hashed,
            string other => other == Hashed,
            _ => false,
        };

        public override int GetHashCode() => Hashed.GetHashCode();

        public static bool operator ==(HashedId left, HashedId right) => left.Equals(right);

        public static bool operator !=(HashedId left, HashedId right) => !(left == right);
    }

    public class InvalidHashedIdException : Exception
    {
        public InvalidHashedIdException()
        {
        }

        public InvalidHashedIdException(string? hashValue) : base($"Invalid hashed ID value '{hashValue}'")
        {
        }

        public InvalidHashedIdException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}