using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.HashingService;
using static SFA.DAS.ApprenticeCommitments.Web.UnitTests.EqualityTests;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class HashedIdTests
    {
        private const string alphabet = "abcdefgh12345678";
        public static IHashingService Hashing = new HashingService.HashingService(alphabet, "testing");

        public static HashedId NewHashedId(string value)
            => HashedId.Create(value, Hashing);

        public static HashedId NewHashedId(long value)
            => HashedId.Create(Hashing.HashValue(value), Hashing);

        [TestCase(1, "42g87g")]
        public void Create_from_id(long id, string hash)
            => NewHashedId(id).Should().BeEquivalentTo(new
            {
                Id = id,
                Hashed = hash,
            });

        [TestCaseSource(nameof(ValidIds))]
        public void Create_from_hash(long id, string hash)
            => NewHashedId(hash).Should().BeEquivalentTo(new
            {
                Id = id,
                Hashed = hash,
            });

        [TestCase("bob")]
        [TestCase("53g26i")]
        [TestCase("4674d55438246e215317")]
        public void Cannot_create_from_invalid_hash(string value)
            => value.Invoking(NewHashedId).Should()
               .Throw<InvalidHashedIdException>()
               .WithMessage($"Invalid hashed ID value '{value}'");

        [TestCaseSource(nameof(ValidIds))]
        public void Equality(long _, string hashedId)
            => TestEqualObjects(NewHashedId(hashedId), NewHashedId(hashedId));

        [Test, AutoData]
        public void Inequality(long a, long b)
            => TestUnequalObjects(NewHashedId(a), NewHashedId(b));

        [TestCaseSource(nameof(ValidIds))]
        public void Nullability(long _, string hashedId)
            => TestAgainstNull(NewHashedId(hashedId));

        [Test, AutoData]
        public void EqualityTestsAgainstBuiltInClass(string value, string other)
        {
            TestEqualObjects(value, new string(value));
            TestUnequalObjects(value, other);
            TestAgainstNull(value);
        }

        [TestCaseSource(nameof(ValidIds))]
        public void Equality_to_string(long _, string hashedId)
            => NewHashedId(hashedId).Should().Be(hashedId);

        [Test, AutoData]
        public void Inequality_to_string(long a, string b)
            => NewHashedId(a).Should().NotBe(b);

        [TestCaseSource(nameof(ValidIds))]
        public void ToString_is_the_hashed_id(long _, string hashedId)
            => NewHashedId(hashedId).ToString().Should().Be(hashedId);

        private static readonly object[] ValidIds =
        {
            new object[] { 1, Hashing.HashValue(1)},
            new object[] { 1234, Hashing.HashValue(1234) },
            new object[] { 9999, Hashing.HashValue(9999) },
            new object[] { 9999999999, Hashing.HashValue(9999999999) },
        };
    }
}