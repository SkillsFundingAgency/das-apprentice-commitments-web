using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using System.Net;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
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

        [Given(@"there is one apprenticeship")]
        public void GivenThereIsOneApprenticeship()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.RegistrationId}/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new[]
                    {
                        new { Id = 1235 },
                    }));

            _context.OuterApi.MockServer.Given(
               Request.Create()
                   .UsingGet()
                   .WithPath($"/apprentices/{_userContext.RegistrationId}/apprenticeships/{_apprenticeshipId}")
                                             )
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBodyAsJson(new
                   {
                       Id = 1235,
                   }));
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