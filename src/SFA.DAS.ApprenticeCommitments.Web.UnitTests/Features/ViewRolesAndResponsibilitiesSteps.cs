using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "ViewRolesAndResponsibilities")]
    public class ViewRolesAndResponsibilitiesSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private long _revisionId;

        public ViewRolesAndResponsibilitiesSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _revisionId = 6615;

            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingAnyMethod())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given(@"the apprentice has verified they have read the page")]
        public void GivenTheApprenticeHasNotVerifiedTheyHaveReadThePage()
        {
            SetupApiConfirmation();
        }

        private void SetupApiConfirmation()
        {
            _context.OuterApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/revisions/{_revisionId}"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new Apprenticeship
                    {
                        Id = _apprenticeshipId.Id,
                        RolesAndResponsibilitiesConfirmations =
                            RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed |
                            RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed |
                            RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed
                    }));
        }

        [When(@"accessing the Roles and responsibilities page from the my apprenticeship page")]
        public async Task WhenAccessingTheRoleAndResponsibilitiesPageFromTheMyApprenticeshipPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/RolesAndResponsibilities?revisionId={_revisionId}");
        }

        [Then(@"the response status code should be OK")]
        public void ThenTheResponseStatusCodeShouldBeOK()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the back link is pointing to the my apprenticeship page")]
        public void ThenTheBackLinkIsPointingToTheConfirmPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<RolesAndResponsibilitiesModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }
    }
}