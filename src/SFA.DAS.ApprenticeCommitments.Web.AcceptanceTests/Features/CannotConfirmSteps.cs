using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "CannotConfirm")]
    public class CannotConfirmSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private readonly long _apprenticeshipId;
        private readonly string _hashedApprenticeshipId;
        private readonly string _backlink;

        public CannotConfirmSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = 1235;
            _hashedApprenticeshipId = _context.Hashing.HashValue(_apprenticeshipId);
            _backlink = $"/apprenticeships/{_hashedApprenticeshipId}";
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.RegistrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.RegistrationId.ToString());
        }

        [When(@"accessing the CannotConfirm page")]
        public async Task WhenAccessingTheCannotConfirm()
        {
            await _context.Web.Get($"/apprenticeships/{_hashedApprenticeshipId}/cannotconfirm");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the backlink is pointing to the confirm page")]
        public void ThenTheBacklinkIsPointingToTheConfirmPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<CannotConfirmApprenticeshipModel>().Which.Backlink.Should().Be(_backlink);
        }
    }
}
