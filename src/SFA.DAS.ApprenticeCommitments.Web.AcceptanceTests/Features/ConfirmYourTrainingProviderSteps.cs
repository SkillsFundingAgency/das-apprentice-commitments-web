using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Collections.Generic;
using System.Linq;
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
        private const string ModelNameKey = nameof(ConfirmYourTrainingModel.TrainingProviderName);
        private const string ModelConfirmedKey = nameof(ConfirmYourTrainingModel.ConfirmedTrainingProvider);

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

            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingAnyMethod()
                         .WithPath("/apprentices/*/apprenticeships/*/trainingproviderconfirmation"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200));
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

        [Given(@"the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _trainingProviderNameConfirmed = null;
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
                    { ModelNameKey, _trainingProviderName },
                    { ModelConfirmedKey, _trainingProviderNameConfirmed.ToString() }
                }));
        }

        [Then("the response status code should be OK")]
        public void ThenTheResponseStatusCodeShouldBeOK()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprenticeship is updated to show the confirmation")]
        public void ThenTheApprenticeshipIsUpdatedToShowTheConfirmation()
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/trainingproviderconfirmation")
                    .UsingPost());

            updates.Should().HaveCount(1);
            
            var post = updates.First();

            JsonConvert
                .DeserializeObject<TrainingProviderConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { ConfirmedTrainingProvider = true, });
        }

        [Then("the apprentice should see the training provider's name")]
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
                .BackLink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
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

        [Then(@"the model should contain an error message")]
        public void ThenTheModelShouldContainAnErrorMessage()
        {
            var model = _context.ActionResult
                .LastActionResult.Should().BeOfType<PageResult>()
                .Which.Model.Should().BeOfType<ConfirmYourTrainingModel>().Subject;

            model.ModelState.ContainsKey(ModelConfirmedKey).Should().BeTrue();
            model.ModelState[ModelConfirmedKey].Errors.Count.Should().Be(1);
            model.ModelState[ModelConfirmedKey].Errors[0].ErrorMessage.Should().Be("Select an answer");
        }
    }
}