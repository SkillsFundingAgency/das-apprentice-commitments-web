using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.Encoding;
using static SFA.DAS.ApprenticeCommitments.Web.UnitTests.EqualityTests;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class HashedIdTests
    {
        private const string alphabet = "abcdefgh12345678";

        public static IEncodingService Hashing = new EncodingService(new EncodingConfig
        {
            Encodings = new List<Encoding.Encoding>
            {
                new Encoding.Encoding
                {
                    Alphabet = alphabet, 
                    EncodingType = "ApprenticeshipId",
                    MinHashLength = 6, 
                    Salt = "Salt"
                }
            }
        });

        public static HashedId NewHashedId(string value)
            => HashedId.Create(value, Hashing);

        public static HashedId NewHashedId(long value)
            => HashedId.Create(Hashing.Encode(value, EncodingType.ApprenticeshipId), Hashing);

        [TestCase(1, "4b8bg8")]
        public void Create_from_id(long id, string hash)
        {
            NewHashedId(id).Should().BeEquivalentTo(new
            {
                Id = id,
                Hashed = hash,
            });
        }

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
            new object[] { 1, Hashing.Encode(1, EncodingType.ApprenticeshipId)},
            new object[] { 1234, Hashing.Encode(1234, EncodingType.ApprenticeshipId) },
            new object[] { 9999, Hashing.Encode(9999, EncodingType.ApprenticeshipId) },
            new object[] { 999999, Hashing.Encode(999999, EncodingType.ApprenticeshipId) },
        };
    }
}