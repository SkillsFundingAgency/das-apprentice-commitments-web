using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmYourTrainingProvider")]
    public class ConfirmYourTrainingProviderSteps : StepsBase
    {
        private readonly TestContext _context;
        private HashedId _apprenticeshipId;
        private string _trainingProviderName;
        private bool? _trainingProviderNameConfirmed;

        public ConfirmYourTrainingProviderSteps(TestContext context) : base(context)
        {
            _context = context;
            _apprenticeshipId = HashedId.Create(1397, _context.Hashing);
            _trainingProviderName = "My Test Company";

            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new
                        {
                            _apprenticeshipId.Id,
                            TrainingProviderName = _trainingProviderName
                        }));
        }

        [Given("the apprentice has not verified their training provider")]
        public void GivenTheApprenticeHasNotVerifiedTheirTrainingProvider()
        {
        }

        [Given("the apprentice confirms their training provider")]
        public void GivenTheApprenticeConfirmsTheirEmployer()
        {
            _trainingProviderNameConfirmed = true;
        }

        [Given("the apprentice states this is not their training provider")]
        public void GivenTheApprenticeStatesThisIsNotTheirTrainingProvider()
        {
            _trainingProviderNameConfirmed = false;
        }

        [When("accessing the ConfirmYourTrainingProvider page")]
        public async Task WhenAccessingTheConfirmYourTrainingProviderPage()
        {
            await _context.Web
                .Get($"/apprenticeships/{_apprenticeshipId.Hashed}/confirmyourtrainingprovider");
        }

        [When("submitting the ConfirmYourTrainingProvider page")]
        public async Task WhenSubmittingTheConfirmYourTrainingProviderPage()
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/confirmyourtrainingprovider",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "TrainingProviderName", _trainingProviderName },
                    { "ConfirmTrainingProvider", _trainingProviderNameConfirmed.ToString() }
                }));
        }

        [Then("the response status code should be OK")]
        public void ThenTheResponseStatusCodeShouldBeOK()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the training provider name")]
        public void ThenTheApprenticeShouldSeeTheTrainingProviderName()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model
                .Should().BeOfType<ConfirmYourTrainingModel>()
                .Which.TrainingProviderName.Should().Be(_trainingProviderName);
        }

        [Then("the back link is pointing to the My Apprenticships page")]
        public void ThenTheBackLinkIsPointingToTheMyApprenticeshipsPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<ConfirmYourTrainingModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }

        [Then("the user should be redirected back to the My Apprenticeships page")]
        public void ThenTheUserShouldBeRedirectedBackToTheOverviewPage()
        {
            var redirect = _context.ActionResult
                .LastActionResult.Should().BeOfType<RedirectToPageResult>().Which;
            redirect.PageName.Should().Be("Confirm");
            redirect
                .RouteValues.Should().ContainKey("ApprenticeshipId")
                .WhichValue.Should().Be(_apprenticeshipId.Hashed);
        }

        [Then("the user should be redirected to the cannot confirm apprenticeship page")]
        public void ThenTheUserShouldBeRedirectedToTheCannotConfirmApprenticeshipPage()
        {
            var redirect = _context.ActionResult
                .LastActionResult.Should().BeOfType<RedirectToPageResult>().Which;
            redirect.PageName.Should().Be("CannotConfirm");
            redirect
                .RouteValues.Should().ContainKey("ApprenticeshipId")
                .WhichValue.Should().Be(_apprenticeshipId.Hashed);
        }
    }
}