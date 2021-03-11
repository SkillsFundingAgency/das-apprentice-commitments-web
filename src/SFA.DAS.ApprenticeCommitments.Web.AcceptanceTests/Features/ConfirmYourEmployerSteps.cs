using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmYourEmployer")]
    public class ConfirmYourEmployerSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private ConfirmYourEmployerModel _confirmYourEmployerModel;
        private long _apprenticeshipId;
        private string _hashedApprenticeshipId;
        private string _employerName;
        private string _backlink;

        public ConfirmYourEmployerSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = 1235;
            _hashedApprenticeshipId = _context.Hashing.HashValue(_apprenticeshipId);
            _employerName = "My Test Company";
            _backlink = $"/apprenticeships/{_hashedApprenticeshipId}/confirm";

            _context.OuterApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson( new { Id = _apprenticeshipId, EmployerName = _employerName }));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.RegistrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.RegistrationId.ToString());
        }

        [Given(@"the apprentice has not verified their employer")]
        public void GivenTheApprenticeHasNotVerifiedTheirEmployer()
        {

        }

        [When(@"accessing the ConfirmYourEmployer page")]
        public async Task WhenAccessingTheConfirmYourEmployerPage()
        {
            await _context.Web.Get($"/apprenticeships/{_hashedApprenticeshipId}/confirmyouremployer");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the apprentice should see the employers name")]
        public void ThenTheApprenticeShouldSeeTheEmployersName()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.EmployerName.Should().Be(_employerName);
        }

        [Then(@"the link is pointing to the confirm page")]
        public void ThenTheLinkIsPointingToTheConfirmPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.Backlink.Should().Be(_backlink);
        }
    }
}