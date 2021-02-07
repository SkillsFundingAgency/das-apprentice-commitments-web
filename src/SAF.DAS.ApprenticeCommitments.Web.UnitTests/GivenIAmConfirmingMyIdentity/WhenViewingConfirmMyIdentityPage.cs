using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SAF.DAS.ApprenticeCommitments.Web.UnitTests.AutoFixtureCustomisations;
using SFA.DAS.ApprenticeCommitments.Web;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.GivenIAmConfirmingMyIdentity
{
    public class WhenViewingConfirmMyIdentityPage
    {
        [Test, PageAutoData]
        public async Task The_page_shows_my_email_address(
            [Frozen] Mock<ApiClient> api,
            [Frozen] ClaimsPrincipal user,
            ConfirmYourIdentityModel sut,
            Registration registration)
        {
            user.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim("sub", registration.Id.ToString())
            }));
            registration.UserId = null;
            api.Setup(x => x.GetRegistration(registration.Id)).Returns(Task.FromResult(registration));

            await sut.OnGetAsync();

            sut.EmailAddress.Should().Be(registration.EmailAddress);
        }
    }
}
