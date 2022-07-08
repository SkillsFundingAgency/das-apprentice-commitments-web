using FluentAssertions;
using Newtonsoft.Json.Linq;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "MyApprenticeshipView")]
    public class MyApprenticeViewSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private Apprenticeship _apprenticeship;
        private HashedId _apprenticeshipId;

        public MyApprenticeViewSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            var f = new Fixture();
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _apprenticeship = f.Build<Apprenticeship>().With(x => x.Id, _apprenticeshipId.Id)
                .With(x => x.ConfirmedOn, DateTime.Today).Create();
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given("the apprentice will navigate to the view page")]
        public void GivenTheApprenticeWillNavigateToTheViewPage()
        {
            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/confirmed/latest"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(_apprenticeship)
                    );
        }

        [Given(@"the apprentice will navigate to the view page for invalid apprenticeship")]
        public void GivenTheApprenticeWillNavigateToTheViewPageForInvalidApprenticeship()
        {
            _context.OuterApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/confirmed/latest"))
                .RespondWith(Response.Create()
                    .WithStatusCode(404)
                );
        }

        [When("accessing the view page")]
        public async Task WhenAccessingTheViewPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/view");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the response status code should be internal error")]
        public void ThenTheResponseStatusCodeShouldBeInternalError()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Then("the apprentice should see the apprenticeship view page for the apprenticeship")]
        public void ThenTheApprenticeShouldSeeTheApprenticeshipViewPage()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ViewMyApprenticeshipModel>();
            model.Should().NotBeNull();
            model.LatestConfirmedApprenticeship.Should().BeEquivalentTo(_apprenticeship);
        }
    }
}