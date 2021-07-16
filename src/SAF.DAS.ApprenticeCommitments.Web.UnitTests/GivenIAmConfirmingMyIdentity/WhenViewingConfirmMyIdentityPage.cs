using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.GivenIAmConfirmingMyIdentity
{
    public class WhenViewingConfirmMyIdentityPage
    {
        [Test, PageAutoData]
        public async Task The_page_shows_my_email_address(
            [Frozen] Mock<IOuterApiClient> api,
            ConfirmYourPersonalDetailsModel sut,
            ClaimsPrincipal user,
            VerifyRegistrationResponse registration)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("apprentice_id", registration.ApprenticeId.ToString())
            }));
            registration.HasCompletedVerification = false;
            api.Setup(x => x.GetRegistration(registration.ApprenticeId)).Returns(Task.FromResult(registration));

            await sut.OnGetAsync(new AuthenticatedUser(user));

            sut.EmailAddress.Should().Be(registration.Email);
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_missing(
            ConfirmYourPersonalDetailsModel sut,
            ClaimsPrincipal user)
        {
            sut.Invoking(x => x.OnGetAsync(new AuthenticatedUser(user)))
               .Should().Throw<Exception>().WithMessage("There is no `apprentice_id` claim.");
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_not_a_guid(
            ConfirmYourPersonalDetailsModel sut,
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