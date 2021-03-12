using System.Collections.Generic;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmYourEmployer")]
    public class ConfirmYourEmployerSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private ConfirmYourEmployerModel _confirmYourEmployerModel;
        private long _apprenticeshipId;
        private string _hashedApprenticeshipId;
        private string _employerName;
        private bool? _employerNameConfirmed;
        private string _backlink;

        public ConfirmYourEmployerSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = 1235;
            _hashedApprenticeshipId = _context.Hashing.HashValue(_apprenticeshipId);
            _employerName = "My Test Company";
            _backlink = $"/apprenticeships/{_hashedApprenticeshipId}";

            _context.OuterApi.MockServer.Given(
                    Request.Create()
                        .UsingGet()
                        .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson( new { Id = _apprenticeshipId, EmployerName = _employerName }));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.RegistrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.RegistrationId.ToString());
        }

        [Given(@"the apprentice has not verified their employer")]
        public void GivenTheApprenticeHasNotVerifiedTheirEmployer()
        {
        }

        [Given(@"the apprentice confirms their employer")]
        public void GivenTheApprenticeConfirmsTheirEmployer()
        {
            _employerNameConfirmed = true;
        }

        [Given(@"the apprentice states this is not their employer")]
        public void GivenTheApprenticeStatesThisIsNotTheirEmployer()
        {
            _employerNameConfirmed = false;
        }

        [Given(@"the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _employerNameConfirmed = null;
        }

        [When(@"submitting the ConfirmYourEmployer page")]
        public async Task WhenSubmittingTheConfirmYourEmployerPage()
        {
            await _context.Web.Post($"/apprenticeships/{_hashedApprenticeshipId}/confirmyouremployer",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "EmployerName", _employerName },
                    { "EmployerConfirm", _employerNameConfirmed.ToString() }
                }));
        }

        [When(@"accessing the ConfirmYourEmployer page")]
        public async Task WhenAccessingTheConfirmYourEmployerPage()
        {
            await _context.Web.Get($"/apprenticeships/{_hashedApprenticeshipId}/confirmyouremployer");
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the apprentice should see the employer's name")]
        public void ThenTheApprenticeShouldSeeTheEmployersName()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.EmployerName.Should().Be(_employerName);
        }

        [Then(@"the link is pointing to the confirm page")]
        public void ThenTheLinkIsPointingToTheConfirmPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourEmployerModel>().Which.Backlink.Should().Be(_backlink);
        }

        [Then(@"the user should be redirected back to the overview page")]
        public void ThenTheUserShouldBeRedirectedBackToTheOverviewPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("Confirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_hashedApprenticeshipId);
        }

        [Then(@"the user should be redirected to the cannot confirm apprenticeship page")]
        public void ThenTheUserShouldBeRedirectedToTheCannotConfirmApprenticeshipPage()
        {
            var redirect = _context.ActionResult.LastActionResult as RedirectToPageResult;
            redirect.Should().NotBeNull();
            redirect.PageName.Should().Be("CannotConfirm");
            redirect.RouteValues["ApprenticeshipId"].Should().Be(_hashedApprenticeshipId);
        }

        [Then(@"the model should contain an error message")]
        public void ThenTheModelShouldContainAnErrorMessage()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmYourEmployerModel>();
            model.Should().NotBeNull();
            model.ModelState["EmployerConfirm"].Errors.Count.Should().Be(1);
        }
    }
}