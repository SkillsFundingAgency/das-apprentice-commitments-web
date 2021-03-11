using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.HashingService;
using System;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class HashedIdTests
    {
        private const string alphabet = "abcdefgh12345678";
        public static IHashingService Hashing = new HashingService(alphabet, "testing");

        public static HashedId NewHashedId(string value)
            => new HashedId(value, alphabet);

        public static HashedId NewHashedId(long value)
            => new HashedId(Hashing.HashValue(value), alphabet);

        [TestCaseSource(nameof(ValidIds))]
        public void Create(string hashedId)
            => NewHashedId(hashedId).Should().NotBeNull();

        [Test]
        public void Cannot_create_from_invalid_hash()
        {
            Func<HashedId> act = () => new HashedId("bob");
            act.Should()
               .Throw<InvalidHashedIdException>()
               .WithMessage($"Invalid hashed ID value 'bob'");
        }

        [TestCaseSource(nameof(ValidIds))]
        public void Equality(string hashedId)
            => EqualityTests.TestEqualObjects(NewHashedId(hashedId), NewHashedId(hashedId));

        [Test, AutoData]
        public void Inequality(long a, long b)
            => EqualityTests.TestUnequalObjects(NewHashedId(a), NewHashedId(b));

        [TestCaseSource(nameof(ValidIds))]
        public void Nullability(string hashedId)
            => EqualityTests.TestAgainstNull(NewHashedId(hashedId));

        [TestCaseSource(nameof(ValidIds))]
        public void Equality_to_string(string hashedId)
            => NewHashedId(hashedId).Should().Be(hashedId);

        [Test, AutoData]
        public void Inequality_to_string(long a, string b)
            => NewHashedId(a).Should().NotBe(b);

        private static readonly object[] ValidIds =
        {
            new object[] { Hashing.HashValue(1234) },
            new object[] { Hashing.HashValue(9999) },
            new object[] { Hashing.HashValue(0) },
            new object[] { Hashing.HashValue(long.MaxValue) },
            new object[] { Hashing.HashValue(long.MinValue) },
        };
    }
}