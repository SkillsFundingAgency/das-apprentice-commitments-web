using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "ViewHowYourApprenticeshipWillBeDelivered")]
    public class ViewHowYourApprenticeshipWillBeDeliveredSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private long _revisionId;

        public ViewHowYourApprenticeshipWillBeDeliveredSteps(TestContext context, RegisteredUserContext userContext) : base(context)
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
            SetupApiConfirmation(true);
        }

        private void SetupApiConfirmation(bool? confirmed)
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/revisions/{_revisionId}"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new { Id = _apprenticeshipId.Id, HowApprenticeshipDeliveredCorrect = confirmed }));
        }

        [When(@"accessing the How your apprenticeship will be delivered page from the my apprenticeship page")]
        public async Task WhenAccessingTheHowYourApprenticeshipWillBeDeliveredPageFromTheMyApprenticeshipPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/howyourapprenticeshipwillbedelivered?revisionId={_revisionId}");
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
                .Model.Should().BeOfType<HowYourApprenticeshipWillBeDeliveredModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }
    }
}