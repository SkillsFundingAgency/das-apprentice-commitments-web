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
    [Scope(Feature = "ConfirmRolesAndResponsibilities")]
    public class ConfirmRolesAndResponsibilitiesSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private long _commitmentStatementId;
        private bool? _rolesAndResponsibilitiesConfirmed;

        public ConfirmRolesAndResponsibilitiesSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _commitmentStatementId = 6613;

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

        [Given(@"the apprentice has not verified their Roles and Responsibilities")]
        public void GivenTheApprenticeHasNotVerifiedTheirRolesAndResponsibilities()
        {
            SetupApiConfirmation(null);
        }

        [Given("the apprentice has negatively confirmed their Roles and Responsibilities")]
        public void GivenTheApprenticeHasNegativelyConfirmedTheirRolesAndResponsibilities()
        {
            SetupApiConfirmation(false);
        }

        private void SetupApiConfirmation(bool? confirmed)
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
                            RolesAndResponsibilitiesCorrect = confirmed
                        }));
        }

        [When(@"accessing the RolesAndResponsibilities page")]
        public async Task WhenAccessingTheRolesAndResponsibilitiesPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/rolesandresponsibilities");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the apprentice should see the Roles and Responsibilities")]
        public void ThenTheApprenticeShouldSeeTheRolesAndResponsibilities()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<RolesAndResponsibilitiesModel>();
        }

        [Then("the user should see the confirmation options")]
        public void ThenTheUserShouldSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<RolesAndResponsibilitiesModel>().Which.RolesAndResponsibilitiesConfirmed.Should().BeNull();
        }

        [Then(@"the link is pointing to the confirm page")]
        public void ThenTheLinkIsPointingToTheConfirmPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<RolesAndResponsibilitiesModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }

        [Given(@"the apprentice confirms their Roles and Responsibilities")]
        public void GivenTheApprenticeConfirmsTheirRolesAndResponsibilities()
        {
            _rolesAndResponsibilitiesConfirmed = true;
        }

        [When(@"submitting the RolesAndResponsibilities page")]
        public async Task WhenSubmittingTheConfirmYourEmployerPage()
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/rolesandresponsibilities",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { nameof(RolesAndResponsibilitiesModel.CommitmentStatementId), _commitmentStatementId.ToString() },
                    { nameof(RolesAndResponsibilitiesModel.RolesAndResponsibilitiesConfirmed), _rolesAndResponsibilitiesConfirmed.ToString() }
                }));
        }

        [Then(@"the apprenticeship is updated to show the a '(.*)' confirmation")]
        public void ThenTheApprenticeshipIsUpdatedToShowTheConfirmation(bool confirm)
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/{_commitmentStatementId}/rolesandresponsibilitiesconfirmation")
                    .UsingPost());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert.DeserializeObject<RolesAndResponsibilitiesConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { RolesAndResponsibilitiesCorrect = confirm });
        }

        [Then(@"the user should be redirected back to the overview page")]
        public void ThenTheUserShouldBeRedirectedBackToTheOverviewPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("Confirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Given(@"the apprentice refuses to confirm their Roles and Responsibilities")]
        public void GivenTheApprenticeRefusesToConfirmTheirRolesAndResponsibilities()
        {
            _rolesAndResponsibilitiesConfirmed = false;
        }

        [Then(@"the user should be redirected to the cannot confirm apprenticeship page")]
        public void ThenTheUserShouldBeRedirectedToTheCannotConfirmApprenticeshipPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("CannotConfirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Given(@"the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _rolesAndResponsibilitiesConfirmed = null;
        }

        [Then(@"the model should contain an error message")]
        public void ThenTheModelShouldContainAnErrorMessage()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<RolesAndResponsibilitiesModel>();
            model.Should().NotBeNull();
            model.ModelState["RolesAndResponsibilitiesConfirmed"].Errors.Count.Should().Be(1);
        }
    }
}