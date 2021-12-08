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
    [Scope(Feature = "ConfirmYourApprenticeshipDetails")]
    public class ConfirmYourApprenticeshipDetailsSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private readonly HashedId _apprenticeshipId;
        private long _revisionId;
        private readonly string _courseName;
        private readonly int _courseLevel;
        private readonly string _courseOption;
        private readonly int _courseDuration;
        private readonly DateTime _plannedStartDate;
        private readonly DateTime _plannedEndDate;
        private bool? _confirmedApprenticeshipDetails;

        public ConfirmYourApprenticeshipDetailsSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
            _revisionId = 6616;

            _courseName = "My Test Course Name";
            _courseLevel = 3;
            _courseOption = (string)null;
            _courseDuration = 19;
            _plannedStartDate = new DateTime(2021, 03, 12);
            _plannedEndDate = new DateTime(2022, 09, 15);

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

        [Given("the apprentice has not verified their apprenticeship details")]
        public void GivenTheApprenticeHasNotVerifiedTheirApprenticeshipDetails()
        {
            SetupApiGetConfirmation(null);
        }

        [Given("the apprentice has negatively confirmed their apprenticeship details")]
        public void GivenTheApprenticeHasNegativelyConfirmedTheirApprenticeshipDetails()
        {
            SetupApiGetConfirmation(false);
        }

        private void SetupApiGetConfirmation(bool? confirmed)
        {
            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingAnyMethod()
                         .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new
                        {
                            _apprenticeshipId.Id,
                            CommitmentStatementId = _revisionId,
                            CourseName = _courseName,
                            CourseLevel = _courseLevel,
                            CourseOption = _courseOption,
                            CourseDuration = _courseDuration,
                            PlannedStartDate = _plannedStartDate,
                            PlannedEndDate = _plannedEndDate,
                            ApprenticeshipDetailsCorrect = confirmed,
                        }));
        }

        [Given("the apprentice confirms their apprenticeship details")]
        public void GivenTheApprenticeConfirmsTheirApprenticeshipDetails()
        {
            _confirmedApprenticeshipDetails = true;
        }

        [Given("the apprentice states these are not their apprenticeship details")]
        public void GivenTheApprenticeStatesTheseAreNotTheirApprenticeshipDetails()
        {
            _confirmedApprenticeshipDetails = false;
        }

        [Given("the apprentice doesn't select an option")]
        public void GivenTheApprenticeDoesnTSelectAnOption()
        {
            _confirmedApprenticeshipDetails = null;
        }

        [When("accessing the YourApprenticeshipDetails page")]
        public async Task WhenAccessingTheYourApprenticeshipDetailsPage()
        {
            await _context.Web.Get($"/apprenticeships/{_apprenticeshipId.Hashed}/yourapprenticeshipdetails");
        }

        [When("submitting the YourApprenticeshipDetails page")]
        public async Task WhenSubmittingTheYourApprenticeshipDetailsPage()
        {
            await _context.Web.Post($"/apprenticeships/{_apprenticeshipId.Hashed}/yourapprenticeshipdetails",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { nameof(YourApprenticeshipDetails.RevisionId), _revisionId.ToString() },
                    { nameof(YourApprenticeshipDetails.CourseName), _courseName },
                    { nameof(YourApprenticeshipDetails.CourseLevel) , _courseLevel.ToString() },
                    { nameof(YourApprenticeshipDetails.CourseOption) , _courseOption },
                    { nameof(YourApprenticeshipDetails.CourseDuration) , _courseDuration.ToString() },
                    { nameof(YourApprenticeshipDetails.PlannedStartDate) , _plannedStartDate.ToString("o")},
                    { nameof(YourApprenticeshipDetails.PlannedEndDate) , _plannedEndDate.ToString("o") },
                    { nameof(YourApprenticeshipDetails.Confirmed), _confirmedApprenticeshipDetails.ToString() }
                }));
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the course name")]
        public void ThenTheApprenticeShouldSeeTheCourseName()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.CourseName.Should().Be(_courseName);
        }

        [Then("the apprentice should see the course level")]
        public void ThenTheApprenticeShouldSeeTheCourseLevel()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.CourseLevel.Should().Be(_courseLevel);
        }

        [Then("the apprentice should see the course option")]
        public void ThenTheApprenticeShouldSeeTheCourseOption()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.CourseOption.Should().Be(_courseOption);
        }

        [Then("the apprentice should see the duration in months")]
        public void ThenTheApprenticeShouldSeeTheDurationInMonths()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.CourseDuration.Should().Be(_courseDuration);
        }

        [Then("the apprentice should see the planned start date")]
        public void ThenTheApprenticeShouldSeeThePlannedStartDate()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.PlannedStartDate.Should().Be(_plannedStartDate);
        }

        [Then("the apprentice should see the planned end date")]
        public void ThenTheApprenticeShouldSeeThePlannedEndDate()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.PlannedEndDate.Should().Be(_plannedEndDate);
        }

        [Then("the user should see the confirmation options")]
        public void ThenTheUserShouldSeeTheConfirmationOptions()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<YourApprenticeshipDetails>().Which.ShowForm.Should().BeTrue();
        }

        [Then("the link is pointing to the confirm page")]
        public void ThenTheLinkIsPointingToTheConfirmPage()
        {
            _context.ActionResult.LastPageResult
                .Model.Should().BeOfType<YourApprenticeshipDetails>().Which
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

        [Then("the apprenticeship is updated to show a '(.*)' confirmation")]
        public void ThenTheApprenticeshipIsUpdatedToShowAConfirmation(bool confirm)
        {
            var updates = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/apprentices/*/apprenticeships/{_apprenticeshipId.Id}/revisions/{_revisionId}/confirmations")
                    .UsingPatch());

            updates.Should().HaveCount(1);

            var post = updates.First();

            JsonConvert
                .DeserializeObject<ApprenticeshipConfirmationRequest>(post.RequestMessage.Body)
                .Should().BeEquivalentTo(new { ApprenticeshipDetailsCorrect = confirm });
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
            var model = _context.ActionResult.LastPageResult.Model.As<YourApprenticeshipDetails>();
            model.Should().NotBeNull();
            model.ModelState["Confirmed"].Errors.Count.Should().Be(1);
        }
    }
}