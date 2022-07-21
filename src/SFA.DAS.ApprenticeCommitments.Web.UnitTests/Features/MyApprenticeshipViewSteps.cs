using AutoFixture;
using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        [Given("the apprentice has changed employer")]
        public void GivenTheApprenticeHasChangedEmployer()
        {
            _apprenticeship.Revisions.Add(new Revision("You started with a new employer", "From a to b", DateTime.Now));
        }

        [Given("the apprentice has changed provider")]
        public void GivenTheApprenticeHasChangedProvider()
        {
            _apprenticeship.Revisions.Add(new Revision("You started with a new training provider", "From a to b", DateTime.Now));
        }

        [Given("the apprentice has changed delivery model")]
        public void GivenTheApprenticeHasChangedDeliveryModel()
        {
            _apprenticeship.Revisions.Add(new Revision("Delivery model changed", "From a to b", DateTime.Now));
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

        [Then("the response should indicate a change of employer")]
        public void ThenTheResponseShouldIndicateAChangeOfEmployer()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ViewMyApprenticeshipModel>();
            model.Should().NotBeNull();
            model.LatestConfirmedApprenticeship.Revisions.Any(x => x.Heading == "You started with a new employer").Should().BeTrue();
        }

        [Then("the response should indicate a change of provider")]
        public void ThenTheResponseShouldIndicateAChangeOfProvider()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ViewMyApprenticeshipModel>();
            model.Should().NotBeNull();
            model.LatestConfirmedApprenticeship.Revisions.Any(x => x.Heading == "You started with a new training provider").Should().BeTrue();
        }

        [Then("the response should indicate a change of delivery model")]
        public void ThenTheResponseShouldIndicateAChangeOfDeliveryModel()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ViewMyApprenticeshipModel>();
            model.Should().NotBeNull();
            model.LatestConfirmedApprenticeship.Revisions.Any(x => x.Heading == "Delivery model changed").Should().BeTrue();
        }

        [Then(@"the revisionId should be specified")]
        public void ThenTheRevisionIdShouldBeSpecified()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ViewMyApprenticeshipModel>();
            model.RevisionId.Should().Be(_apprenticeship.RevisionId);
        }
    }
}