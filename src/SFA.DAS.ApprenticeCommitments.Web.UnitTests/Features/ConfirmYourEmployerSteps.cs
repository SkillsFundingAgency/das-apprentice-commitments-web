using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmYourEmployer")]
    public class ConfirmYourEmployerSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private readonly HashedId _apprenticeshipId;
        private readonly long _revisionId;
        private readonly string _employerName;
        private string _deliveryModel;
        private bool? _employerNameConfirmed;

        public ConfirmYourEmployerSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _revisionId = 6612;
            _employerName = "My Test Company";
            _deliveryModel = DeliveryModel.Regular.ToString();

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

        [Given("the apprentice has not verified their employer")]
        public void GivenTheApprenticeHasNotVerifiedTheirEmployer()
        {
            SetupApiGetEmployerConfirmation(confirmed: null);
        }

        [Given("the apprentice has confirmed this is not their employer")]
        public void GivenTheApprenticeHasConfirmedThisIsNotTheirEmployer()
        {
            SetupApiGetEmployerConfirmation(confirmed: false);
        }

        [Given("the apprentice has confirmed this is their employer")]
        public void GivenTheApprenticeHasConfirmedThisIsTheirEmployer()
        {
            SetupApiGetEmployerConfirmation(confirmed: true);
        }

        [Given(@"the apprentice has confirmed everything")]
        public void GivenTheApprenticeHasConfirmedEverything()
        {
            SetupApiGetEmployerConfirmation(confirmed: true, everythingConfirmed: true);
        }

        private void SetupApiGetEmployerConfirmation(bool? confirmed, bool? everythingConfirmed = null)
        {
            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new
                        {
                            _apprenticeshipId.Id,
                            CommitmentStatementId = _revisionId,
                            DeliveryModel = _deliveryModel,
                            EmployerName = _employerName,
                            EmployerCorrect = confirmed,
                            TrainingProviderCorrect = everythingConfirmed,
                            RolesAndResponsibilitiesCorrect = everythingConfirmed,
                            ApprenticeshipDetailsCorrect = everythingConfirmed,
                            HowApprenticeshipDeliveredCorrect = everythingConfirmed,
                            ConfirmedOn = everythingConfirmed == true ? (DateTime?)DateTime.Now : null
                        }));
        }

        [Given(@"the apprentice is on a flexi-job apprenticeship")]
        public void GivenTheApprenticeIsOnAFlexi_JobApprenticeship()
        {
            _deliveryModel = DeliveryModel.PortableFlexiJob.ToString();
        }

        [Given("the apprentice confirms their employer")]
        public void GivenTheApprenticeConfirmsTheirEmployer()
        {
            _employerNameConfirmed = true;
        }

        [Given("the apprentice states this is not their employer")]
        public void GivenTheApprenticeStatesThisIsNotTheirEmployer()
        {
            _employerNameConfirmed = false;
        }

        [Given("the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _employerNameConfirmed = null;
        }

        [When("submitting the ConfirmYourEmployer page")]
        public async Task WhenSubmittingTheConfirmYourEmployerPage()
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/confirmyouremployer",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { nameof(ConfirmYourEmployerModel.RevisionId), _revisionId.ToString() },
                    { nameof(ConfirmYourEmployerModel.EmployerName), _employerName },
                    { nameof(ConfirmYourEmployerModel.Confirmed), _employerNameConfirmed.ToString() }
                }));
        }

        [When("accessing the ConfirmYourEmployer page")]
        public async Task WhenAccessingTheConfirmYourEmployerPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/confirmyouremployer");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the employer's name")]
        public void ThenTheApprenticeShouldSeeTheEmployersName()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.EmployerName.Should().Be(_employerName);
        }

        [Then(@"the delivery model is ""(.*)""")]
        public void ThenTheDeliveryModelIs(string deliveryModel)
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.DeliveryModel.ToString().Should().Be(deliveryModel);
        }

        [Then("the user should see the confirmation options")]
        public void ThenTheUserShouldSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.ShowForm.Should().BeTrue();
        }

        [Then("the user should not see the confirmation options")]
        public void ThenTheUserShouldNotSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.ShowForm.Should().BeFalse();
        }

        [Then("the link is pointing to the confirm page")]
        public void ThenTheLinkIsPointingToTheConfirmPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<ConfirmYourEmployerModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }

        [Then("the user should be redirected back to the overview page")]
        public void ThenTheUserShouldBeRedirectedBackToTheOverviewPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("Confirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Then("the apprenticeship is updated to show the a '(.*)' confirmation")]
        public void ThenTheApprenticeshipIsUpdatedToShowTheAConfirmation(bool confirm)
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/revisions/{_revisionId}/confirmations")
                    .UsingPatch());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert
                .DeserializeObject<ApprenticeshipConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { EmployerCorrect = confirm });
        }

        [Then("the user should be redirected to the cannot confirm apprenticeship page")]
        public void ThenTheUserShouldBeRedirectedToTheCannotConfirmApprenticeshipPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("CannotConfirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Then("the model should contain an error message")]
        public void ThenTheModelShouldContainAnErrorMessage()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmYourEmployerModel>();
            model.Should().NotBeNull();
            model.ModelState["Confirmed"].Errors.Count.Should().Be(1);
        }

        [Then(@"the user should be able to change their answer")]
        public void ThenTheUserShouldBeAbleToChangeTheirAnswer()
        {
            _context.ActionResult.LastPageResult.Model
                .Should().BeOfType<ConfirmYourEmployerModel>()
                .Which.CanChangeAnswer.Should().BeTrue();
        }

        [Then(@"the user should not be able to change their answer")]
        public void ThenTheUserShouldNotBeAbleToChangeTheirAnswer()
        {
            _context.ActionResult.LastPageResult.Model
                .Should().BeOfType<ConfirmYourEmployerModel>()
                .Which.CanChangeAnswer.Should().BeFalse();
        }
    }
}