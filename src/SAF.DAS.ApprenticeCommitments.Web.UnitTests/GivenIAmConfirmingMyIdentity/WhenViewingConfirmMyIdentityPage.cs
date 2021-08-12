using FluentAssertions;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System;
using System.Security.Claims;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.GivenIAmConfirmingMyIdentity
{
    public class WhenViewingConfirmMyIdentityPage
    {
        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_missing(
            AccountModel sut,
            ClaimsPrincipal user)
        {
            sut.Invoking(x => x.OnGetAsync(new AuthenticatedUser(user)))
               .Should().Throw<Exception>().WithMessage("There is no `apprentice_id` claim.");
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_not_a_guid(
            AccountModel sut,
            ClaimsPrincipal user,
            string notAGuid)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("apprentice_id", notAGuid)
            }));

            sut.Invoking(x => x.OnGetAsync(new AuthenticatedUser(user)))
               .Should().Throw<Exception>()
               .WithMessage($"`{notAGuid}` in claim `apprentice_id` is not a valid identifier");
        }
    }
}