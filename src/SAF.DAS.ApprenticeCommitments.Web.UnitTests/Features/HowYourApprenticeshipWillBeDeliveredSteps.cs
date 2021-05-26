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
    [Scope(Feature = "HowYourApprenticeshipWillBeDelivered")]
    public class HowYourApprenticeshipWillBeDeliveredSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private bool? _confirmedHowApprenticeshipDelivered;

        public HowYourApprenticeshipWillBeDeliveredSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);

            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/howapprenticeshipwillbedeliveredconfirmation"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given(@"the apprentice has not verified they have read the page")]
        public void GivenTheApprenticeHasNotVerifiedTheyHaveReadThePage()
        {
            SetupApiConfirmation(null);
        }

        [Given(@"the apprentice confirms they understand what they have read")]
        public void ThenTheApprenticeConfirmsTheyUnderstandWhatTheyHaveRead()
        {
            _confirmedHowApprenticeshipDelivered = true;
        }

        [Given(@"the apprentice states they do not understand what they have read")]
        public void ThenTheApprenticeStatesTheyDoNotUnderstandWhatTheyHaveRead()
        {
            _confirmedHowApprenticeshipDelivered = false;
        }

        [Given(@"the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _confirmedHowApprenticeshipDelivered = null;
        }

        [Given(@"the apprentice has negatively confirmed they have read the page")]
        public void GivenTheApprenticeHasNegativelyConfirmedTheyHaveReadThePage()
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
                    .WithBodyAsJson(new { Id = _apprenticeshipId.Id, HowApprenticeshipDeliveredCorrect = confirmed }));
        }

        [When(@"accessing the How your apprenticeship will be delivered page")]
        public async Task WhenAccessingTheHowYourApprenticeshipWillBeDeliveredPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/howyourapprenticeshipwillbedelivered");
        }

        [Then(@"the response status code should be OK")]
        public void ThenTheResponseStatusCodeShouldBeOK()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the back link is pointing to the confirm page")]
        public void ThenTheBackLinkIsPointingToTheConfirmPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<HowYourApprenticeshipWillBeDeliveredModel>().Which
                .Backlink.Should().Be(Urls.MyApprenticshipPage(_apprenticeshipId));
        }

        [When(@"submitting the HowYourApprenticeshipWillBeDelivered page")]
        public async Task WhenSubmittingTheHowYourApprenticeshipWillBeDeliveredPage()
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/howyourapprenticeshipwillbedelivered",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "ConfirmedHowApprenticeshipDelivered", _confirmedHowApprenticeshipDelivered.ToString() }
                }));
        }

        [Then(@"the apprenticeship is updated to show the a '(.*)' confirmation")]
        public void ThenTheApprenticeshipIsUpdatedToShowTheAConfirmation(bool confirm)
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/howapprenticeshipwillbedeliveredconfirmation")
                    .UsingPost());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert.DeserializeObject<HowApprenticeshipDeliveredConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { HowApprenticeshipDeliveredCorrect = confirm });
        }

        [Then("the user should see the confirmation options")]
        public void ThenTheUserShouldSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<HowYourApprenticeshipWillBeDeliveredModel>().Which.ConfirmedHowApprenticeshipDelivered.Should().BeNull();
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
            var model = _context.ActionResult.LastPageResult.Model.As<HowYourApprenticeshipWillBeDeliveredModel>();
            model.Should().NotBeNull();
            model.ModelState["ConfirmedHowApprenticeshipDelivered"].Errors.Count.Should().Be(1);
        }
    }
}