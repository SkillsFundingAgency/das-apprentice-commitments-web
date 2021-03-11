using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Services;
using SFA.DAS.HashingService;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class HashedIdTests
    {
        private const string alphabet = "abcdefgh12345678";
        public static IHashingService Hashing = new HashingService(alphabet, "testing");

        public static HashedId NewHashedId(string value)
            => HashedId.Create(value, Hashing);

        public static HashedId NewHashedId(long value)
            => HashedId.Create(Hashing.HashValue(value), Hashing);

        [TestCaseSource(nameof(ValidIds))]
        public void Create(string hashedId)
            => NewHashedId(hashedId).Should().NotBeNull();

        [TestCase("bob")]
        [TestCase("53g26i")]
        //[TestCase("4674d55438246e215317")]
        public void Cannot_create_from_invalid_hash(string value)
            => value.Invoking(NewHashedId).Should()
               .Throw<InvalidHashedIdException>()
               .WithMessage($"Invalid hashed ID value '{value}'");

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

        [TestCaseSource(nameof(ValidIds))]
        public void ToString_is_the_hashed_id(string hashedId)
            => NewHashedId(hashedId).ToString().Should().Be(hashedId);

        private static readonly object[] ValidIds =
        {
            new object[] { Hashing.HashValue(1) },
            new object[] { Hashing.HashValue(1234) },
            new object[] { Hashing.HashValue(9999) },
            new object[] { Hashing.HashValue(9999999999) },
        };
    }
}