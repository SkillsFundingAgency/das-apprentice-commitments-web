using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Security.Claims;

#nullable enable

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class UserIdentityNameTests
    {
        [TestCase("alice", "armstrong", "alice armstrong")]
        [TestCase("", "bell", "bell")]
        [TestCase("clive", "", "clive")]
        [TestCase("", "", "")]
        public void Joins_names_from_simple_name(string first, string last, string full)
        {
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("given_name", first),
                new Claim("family_name", last),
            }));

            user.FullName().Should().Be(full);
        }

        [TestCase("alice", "armstrong", "alice armstrong")]
        [TestCase("", "bell", "bell")]
        [TestCase("clive", "", "clive")]
        [TestCase("", "", "")]
        public void Joins_names_from_microsoft_name(string first, string last, string full)
        {
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.GivenName, first),
                new Claim(ClaimTypes.Surname, last),
            }));

            user.FullName().Should().Be(full);
        }

        [Test, AutoData]
        public void Joins_names_with_missing_given_name(string family)
        {
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("family_name", family),
            }));

            user.FullName().Should().Be(family);
        }

        [Test, AutoData]
        public void Joins_names_with_missing_family_name(string given)
        {
            var user = new ClaimsPrincipal();
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("given_name", given),
            }));

            user.FullName().Should().Be(given);
        }

        [Test]
        public void Claims_cannot_have_null_value()
        {
            new ClaimsPrincipal().Invoking(x =>
                x.AddIdentity(new ClaimsIdentity(new[]
                {
                    new Claim("type", null),
                })))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
