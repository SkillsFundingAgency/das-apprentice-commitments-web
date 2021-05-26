using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "MyApprenticeships")]
    public class MyApprenticeshipsSteps : StepsBase
    {
        private readonly TestContext _context;
        private HashedId _apprenticeshipId;
        private readonly RegisteredUserContext _userContext;

        public MyApprenticeshipsSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given(@"there is one apprenticeship")]
        public void GivenThereIsOneApprenticeship()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new[]
                    {
                        new { Id = 1235 },
                    }));

            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}/apprenticeships/{_apprenticeshipId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = 1235,
                    }));
        }

        [When(@"accessing the ""(.*)"" page")]
        public async Task WhenAccessingThePage(string page)
        {
            await _context.Web.Get(page);
        }

        [Then("the response should Redirect the apprenticeship page")]
        public void ThenTheResponseStatusCodeShouldBeRedirect()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("/apprenticeships/g3312g");
        }

        [Then(@"the apprentice should see the overview page for their apprenticeship")]
        public void ThenTheApprenticeShouldSeeTheOverviewPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<ConfirmApprenticeshipModel>()
                .Which.ApprenticeshipId.Should().Be(_apprenticeshipId.Hashed);
        }
    }
}