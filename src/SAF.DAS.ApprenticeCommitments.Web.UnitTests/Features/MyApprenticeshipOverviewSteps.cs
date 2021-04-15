using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "MyApprenticeshipOverview")]
    public class MyApprenticeOverviewSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private bool _EmployerConfirmation, _TrainingProviderConfirmation, _ApprenticeshipDetailsConfirmation, _RolesAndResponsibilitiesConfirmation;

        public MyApprenticeOverviewSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [Given("the apprentice will navigate to the overview page")]
        public void GivenTheApprenticeWillNavigateToTheOverviewPage()
        {
            _context.OuterApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new { 
                            Id = _apprenticeshipId.Id,
                            EmployerCorrect = _EmployerConfirmation,
                            TrainingProviderCorrect = _TrainingProviderConfirmation,
                            ApprenticeshipDetailsCorrect = _ApprenticeshipDetailsConfirmation,
                            RolesAndResponsibilitiesCorrect = _RolesAndResponsibilitiesConfirmation
                        }));
        }

        [When(@"accessing the overview page")]
        public async Task WhenAccessingTheOverviewPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Then("the apprentice should see the apprenticeship overview page for the apprenticeship")]
        public void ThenTheApprenticeShouldSeeTheGreenConfirmationBoxWithTheEmpoloyersName()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
        }

        [Given(@"the apprentice has not confirmed every aspect of the apprenciceship")]
        public void GivenTheApprenticeHasNotConfirmedEveryAspectOfTheApprenciceship()
        {
            _EmployerConfirmation =
                _TrainingProviderConfirmation =
                _ApprenticeshipDetailsConfirmation =
                _RolesAndResponsibilitiesConfirmation = false;
        }

        [Then(@"the apprentice should not see the ready to confirm banner")]
        public void ThenTheApprenticeShouldNotSeeTheReadyToConfirmBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.AllConfirmed.Should().Be(false);
        }

        [Given(@"the apprentice has confirmed every aspect of the apprenciceship")]
        public void WhenTheApprenticeHasConfirmedEveryAspectOfTheApprenciceship()
        {
            _EmployerConfirmation =
                _TrainingProviderConfirmation =
                _ApprenticeshipDetailsConfirmation =
                _RolesAndResponsibilitiesConfirmation = true;
        }

        [Then(@"the apprentice should see the ready to confirm banner")]
        public void ThenTheApprenticeShouldSeeTheReadyToConfirmBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.AllConfirmed.Should().Be(true);
        }
    }
}
