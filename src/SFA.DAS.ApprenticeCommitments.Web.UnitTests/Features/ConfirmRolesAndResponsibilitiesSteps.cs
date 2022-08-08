using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities;
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
        private readonly HashedId _apprenticeshipId;
        private readonly long _revisionId;
        private bool _sectionConfirmed;

        public ConfirmRolesAndResponsibilitiesSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _revisionId = 6613;

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
            SetupApiConfirmation(RolesAndResponsibilitiesConfirmations.None);
        }

        [Given(@"the apprentice has confirmed the section (.*)")]
        public void GivenTheApprenticeHasConfirmedTheirApprenticeRolesAndResponsibilities(RolesAndResponsibilitiesConfirmations confirmations)
        {
            SetupApiConfirmation(confirmations);
        }

        [Given(@"the apprentice has verified their Roles and Responsibilities")]
        public void GivenTheApprenticeHasVerifiedTheirRolesAndResponsibilities()
        {
            SetupApiConfirmation(RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed |
                                 RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed |
                                 RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed);
        }

        [Given("the apprentice has partly confirmed their Roles and Responsibilities")]
        public void GivenTheApprenticeHasPartlyConfirmedTheirRolesAndResponsibilities()
        {
            SetupApiConfirmation(RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed);
        }

        private void SetupApiConfirmation(RolesAndResponsibilitiesConfirmations confirmations)
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
                            RolesAndResponsibilitiesConfirmations = confirmations
                        }));
        }

        [When(@"accessing the RolesAndResponsibilities page")]
        public async Task WhenAccessingTheRolesAndResponsibilitiesPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/rolesandresponsibilities/");
        }


        [When(@"accessing the confirm RolesAndResponsibilities\\(.*) page")]
        public async Task WhenAccessingTheConfirmRolesAndResponsibilitiesPage(string section)
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/rolesandresponsibilities/{section}");
        }

        [Given(@"the apprentice is confirming the section with (.*)")]
        public void GivenTheApprenticeIsConfirmingTheSection(bool confirmed)
        {
            _sectionConfirmed = confirmed;
        }

        [When(@"submitting the RolesAndResponsibilities\\(.*) page")]
        public async Task WhenSubmittingTheRolesAndResponsibilitiesPage(string section)
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/rolesandresponsibilities/{section}", 
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { nameof(SectionConfirmationPageModel.RevisionId), _revisionId.ToString() },
                    { nameof(SectionConfirmationPageModel.SectionConfirmed), _sectionConfirmed.ToString() }
                }));
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the response status code should be Redirect")]
        public void ThenTheResponseStatusCodeShouldBeRedirect()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        }

        [Then(@"the redirect address is to the apprentice roles and responsibilities page")]
        public void ThenTheRedirectAddressIsToTheApprenticeRolesAndResponsibilitiesPage()
        {
            var redirect = _context.ActionResult.LastRedirectToPageResult;
            redirect.PageName.Should().Be("1");
        }

        [Then(@"the apprentice should see the Roles and Responsibilities")]
        public void ThenTheApprenticeShouldSeeTheRolesAndResponsibilities()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<RolesAndResponsibilitiesModel>();
        }

        [Then(@"the backlink will return to overview page")]
        public void ThenTheBacklinkWillReturnToOverviewPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<RolesAndResponsibilitiesModel>().Which.Backlink.Should().Be($"/apprenticeships/{_apprenticeshipId.Hashed}");
        }

        [Then(@"the section confirmed checkbox should be already checked")]
        public void ThenTheSectionConfirmedCheckboxShouldBeAlreadyChecked()
        {
            var page = _context.ActionResult.LastPageResult;
            var model = page.Model as SectionConfirmationPageModel;
            model.Should().NotBeNull();
            model.SectionConfirmed.Should().BeTrue();
        }

        [Then(@"backlink with return to (.*)")]
        public void ThenBacklinkWithReturnTo(string backlink)
        {
            backlink = backlink.Replace("?", _apprenticeshipId.Hashed);

            var page = _context.ActionResult.LastPageResult;
            var model = page.Model as SectionConfirmationPageModel;
            model.Should().NotBeNull();
            model.Backlink.Should().Be(backlink);
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
                .Backlink.Should().Be(Urls.ConfirmMyApprenticshipPage(_apprenticeshipId));
        }

        [Then(@"the (.*) should be saved")]
        public void ThenTheShouldBeSaved(RolesAndResponsibilitiesConfirmations confirmation)
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/revisions/{_revisionId}/confirmations")
                    .UsingPatch());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert.DeserializeObject<ApprenticeshipConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { RolesAndResponsibilitiesConfirmations = confirmation });
        }

        [Then(@"the user should be redirected back to the overview page")]
        public void ThenTheUserShouldBeRedirectedBackToTheOverviewPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("Confirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Then(@"the user should be redirected to the cannot confirm apprenticeship page")]
        public void ThenTheUserShouldBeRedirectedToTheCannotConfirmApprenticeshipPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("CannotConfirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_apprenticeshipId.Hashed);
        }

        [Then(@"the model should contain an error message")]
        public void ThenTheModelShouldContainAnErrorMessage()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<RolesAndResponsibilitiesModel>();
            model.Should().NotBeNull();
            model.ModelState["RolesAndResponsibilitiesConfirmed"].Errors.Count.Should().Be(1);
        }

        [Then(@"apprentice is redirected to (.*)")]
        public void ThenApprenticeIsRedirectedTo(string nextPage)
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be(nextPage);
        }
    }
}