using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
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
            _context.ClearCookies();
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
            await _context.Web.FollowLocalRedirects();
        }

        [When("the user attempts to land on the Register page with a registration code")]
        public async Task GivenTheUserAttemptsToLandOnApprenticeshipIndexPageWithARegistrationCode()
        {
            await _context.Web.Get("register/banana");
            await _context.Web.FollowLocalRedirects();
        }

        [When("the user attempts to land on root index page")]
        public async Task GivenTheUserAttemptsToLandOnRootIndexPage()
        {
            await _context.Web.Get("/");
            await _context.Web.FollowLocalRedirects();
        }

        [When("the user attempts to land on personalised page (.*)")]
        public async Task GivenTheUserAttemptsToLandOnAnyPersonalizedApprenticeshipPortalPage(string page)
        {
            await _context.Web.Get($"Apprenticeships/{_context.Hashing.HashValue(_userContext.ApprenticeId)}/{page}");
            await _context.Web.FollowLocalRedirects();
        }

        [Then("redirect the user to the Confirm ID page")]
        public void ThenRedirectTheUserToTheConfirmIDPage()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("/Register");
        }

        [Then("redirect the user to the home page")]
        public void ThenRedirectTheUserToTheHomePage()
        {
            _context.Web.Response.Should().Be302Redirect()
                .And.HaveHeader("Location").And.Match("https://home/Home");
        }

        [Then("redirect the user to the TermsOfUse page")]
        public void ThenRedirectTheUserToTermsOfUse()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.ActionResult.LastRedirectResult.Url.Should().EndWith("//account/TermsOfUse");
        }

        [Then("redirect the user to the home page with a NotMatched banner")]
        public void ThenRedirectTheUserToTheHomePageWithANotMatchedBanner()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            _context.ActionResult.LastPageResult.Should().NotBeNull();
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<CheckYourDetails>();
        }

        [Then("redirect the user to the overview page")]
        public void ThenRedirectTheUserToTheOverviewPage()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            _context.ActionResult.LastPageResult.Should().NotBeNull();
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<ConfirmApprenticeshipModel>();
        }

        [Then("redirect the user to the Account page")]
        public void ThenRedirectTheUserToTheAccountPage()
        {
            _context.Web.Response.Should().Be302Found();
            _context.ActionResult.LastRedirectResult.Url.Should().EndWith("//account/Account");
        }

        [Then("store the registration code in a cookie")]
        public void ThenStoreTheRegistrationCodeInACookie()
        {
            _context.Web.Cookies.GetCookies(_context.Web.BaseAddress).Should().ContainEquivalentOf(new
            {
                Name = "RegistrationCode",
                Value = "banana",
            });
        }
    }
}