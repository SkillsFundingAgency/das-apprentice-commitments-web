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
    [Scope(Feature = "TransactionComplete")]
    public class TransactionCompleteSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;

        public TransactionCompleteSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);

            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new { Id = _apprenticeshipId.Id, CourseName = "Test Course" }));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [When(@"accessing the TransactionComplete page")]
        public async Task WhenAccessingTheTransactionCompletePage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/transactioncomplete");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the Green confirmation box with the course name")]
        public void ThenTheApprenticeShouldSeeTheGreenConfirmationBoxWithTheCourseName()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<TransactionCompleteModel>();
            model.Should().NotBeNull();
            model.CourseName.Should().Be("Test Course");
        }
    }
}