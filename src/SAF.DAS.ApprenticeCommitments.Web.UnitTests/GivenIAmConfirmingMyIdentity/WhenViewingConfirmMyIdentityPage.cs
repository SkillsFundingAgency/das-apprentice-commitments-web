using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web.Api;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.GivenIAmConfirmingMyIdentity
{
    public class WhenViewingConfirmMyIdentityPage
    {
        [Test, PageAutoData]
        public async Task The_page_shows_my_email_address(
            [Frozen] Mock<IApiClient> api,
            ConfirmYourIdentityModel sut,
            ClaimsPrincipal user,
            Registration registration)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("registration_id", registration.Id.ToString())
            }));
            registration.UserId = null;
            api.Setup(x => x.GetRegistration(registration.Id)).Returns(Task.FromResult(registration));

            await sut.OnGetAsync(new RegistrationUser(user));

            sut.EmailAddress.Should().Be(registration.EmailAddress);
        }

        [Test, PageAutoData]
        public void Throws_when_the_registration_claim_is_missing(
            ConfirmYourIdentityModel sut,
            ClaimsPrincipal user)
        {
            sut.Invoking(x => x.OnGetAsync(new RegistrationUser(user)))
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

            sut.Invoking(x => x.OnGetAsync(new RegistrationUser(user)))
                .Should().Throw<Exception>()
                .WithMessage($"`{notAGuid}` in claim `registration_id` is not a valid identifier");
        }
    }
}
