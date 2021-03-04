using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    public class OverviewSteps : StepsBase
    {
        private readonly TestContext _context;
        private long _apprenticeshipId = 1235;
        private readonly RegisteredUserContext _userContext;

        public OverviewSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
        }

        [Given(@"there is one apprenticeship")]
        public void GivenThereIsOneApprenticeship()
        {
            _context.OuterApi.MockServer.Given(
               Request.Create()
                   .UsingGet()
                   .WithPath($"/apprentices/{_userContext.RegistrationId}/currentapprenticeship")
                                             )
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBodyAsJson(new
                   {
                       Id = 1235,
                   }));
        }

        [Then(@"the apprentice should see the overview page for their apprenticeship")]
        public void ThenTheApprenticeShouldSeeTheOverviewPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<OverviewModel>().Which.ApprenticeshipId.Should().Be(_apprenticeshipId);
        }
    }
}