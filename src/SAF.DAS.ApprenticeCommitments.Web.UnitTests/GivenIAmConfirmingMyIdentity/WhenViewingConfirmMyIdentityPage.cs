using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.GivenIAmConfirmingMyIdentity
{
    public class WhenViewingConfirmMyIdentityPage
    {
        [Test, PageAutoData]
        public async Task The_page_shows_my_email_address(
            [Frozen] Mock<IOuterApiClient> api,
            ConfirmYourIdentityModel sut,
            ClaimsPrincipal user,
            VerifyRegistrationResponse registration)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("registration_id", registration.ApprenticeId.ToString())
            }));
            registration.HasCompletedVerification = false;
            api.Setup(x => x.GetRegistration(registration.ApprenticeId)).Returns(Task.FromResult(registration));

            await sut.OnGetAsync(new AuthenticatedUser(user));

            sut.EmailAddress.Should().Be(registration.Email);
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_missing(
            ConfirmYourIdentityModel sut,
            ClaimsPrincipal user)
        {
            sut.Invoking(x => x.OnGetAsync(new AuthenticatedUser(user)))
                .Should().Throw<Exception>().WithMessage("There is no `registration_id` claim.");
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_not_a_guid(
            ConfirmYourIdentityModel sut,
            ClaimsPrincipal user,
            string notAGuid)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("registration_id", notAGuid)
            }));

            sut.Invoking(x => x.OnGetAsync(new AuthenticatedUser(user)))
                .Should().Throw<Exception>()
                .WithMessage($"`{notAGuid}` in claim `registration_id` is not a valid identifier");
        }
    }
}
