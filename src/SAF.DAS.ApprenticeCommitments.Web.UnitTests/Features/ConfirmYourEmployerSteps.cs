using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private HashedId _apprenticeshipId;
        private long _commitmentStatementId;
        private string _employerName;
        private bool? _employerNameConfirmed;

        public ConfirmYourEmployerSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _commitmentStatementId = 6612;
            _employerName = "My Test Company";

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

        private void SetupApiGetEmployerConfirmation(bool? confirmed)
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
                            CommitmentStatementId = _commitmentStatementId,
                            EmployerName = _employerName,
                            EmployerCorrect = confirmed,
                        }));
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
                    { nameof(ConfirmYourEmployerModel.CommitmentStatementId), _commitmentStatementId.ToString() },
                    { nameof(ConfirmYourEmployerModel.EmployerName), _employerName },
                    { nameof(ConfirmYourEmployerModel.ConfirmedEmployer), _employerNameConfirmed.ToString() }
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

        [Then("the user should see the confirmation options")]
        public void ThenTheUserShouldSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.ConfirmedEmployer.Should().BeNull();
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
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/{_commitmentStatementId}/employerconfirmation")
                    .UsingPost());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert.DeserializeObject<EmployerConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { EmployerCorrect = confirm, });
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
            model.ModelState["ConfirmedEmployer"].Errors.Count.Should().Be(1);
        }
    }
}