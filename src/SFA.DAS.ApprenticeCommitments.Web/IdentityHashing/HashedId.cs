using Microsoft.AspNetCore.Mvc;
using SFA.DAS.HashingService;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing
{
    [ModelBinder(typeof(HashedIdModelBinder))]
    public sealed class HashedId
    {
        public long Id { get; }
        public string Hashed { get; }

        public static HashedId Create(int id, IHashingService hashing)
            => new HashedId(id, hashing.HashValue(id));

        public static HashedId Create(string hashed, IHashingService hashing)
        {
            return TryCreate(hashed, hashing, out var hashedId)
                ? hashedId
                : throw new InvalidHashedIdException(hashed);
        }

        public static bool TryCreate(
            string hashed, IHashingService hashing,
            [MaybeNullWhen(false)] out HashedId hashedId)
        {
            if (hashing.TryDecodeValue(hashed, out var id))
            {
                hashedId = new HashedId(id, hashed);
                return true;
            }
            else
            {
                hashedId = default;
                return false;
            }
        }

        private HashedId(long id, string hashed)
        {
            Id = id;
            Hashed = hashed;
        }

        public override bool Equals(object? obj) => obj switch
        {
            HashedId other => other.Hashed == Hashed,
            string other => other == Hashed,
            _ => false,
        };

        public override int GetHashCode() => Hashed.GetHashCode();

        public override string ToString() => Hashed;

        public static bool operator ==(HashedId? left, HashedId? right)
            => (left, right) switch
            {
                (null, null) => true,
                (null, _) => false,
                _ => left!.Equals(right)
            };

        public static bool operator !=(HashedId? left, HashedId? right) => !(left == right);
    }
}