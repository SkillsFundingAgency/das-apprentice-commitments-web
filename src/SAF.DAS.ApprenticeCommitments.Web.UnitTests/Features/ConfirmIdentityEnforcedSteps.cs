using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class ConfirmIdentityEnforcedSteps
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;

        public ConfirmIdentityEnforcedSteps(TestContext context, RegisteredUserContext userContext)
        {
            _userContext = userContext;
            _context = context;
        }

        [When("the user has not already confirmed their identity")]
        public void GivenTheApprenticeHasNotVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/apprentices/*/apprenticeships/{_userContext.ApprenticeId}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new { Id = _userContext.ApprenticeId }));
        }

        [When("the user attempts to land on Apprenticeships index page")]
        public async Task GivenTheUserAttemptsToLandOnApprenticeshipIndexPage()
        {
            await _context.Web.Get("Apprenticeships");
        }

        [When("the user attempts to land on personalised page (.*)")]
        public async Task GivenTheUserAttemptsToLandOnAnyPersonalizedApprenticeshipPortalPage(string page)
        {
            await _context.Web.Get($"Apprenticeships/{_context.Hashing.HashValue(_userContext.ApprenticeId)}/{page}");
        }

        [Then("redirect the user to the Confirm ID page")]
        public void ThenRedirectTheUserToTheConfirmIDPage()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("/ConfirmYourPersonalDetails");
        }
    }
}