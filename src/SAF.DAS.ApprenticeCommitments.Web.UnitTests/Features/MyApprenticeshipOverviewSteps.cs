using FluentAssertions;
using Newtonsoft.Json.Linq;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "MyApprenticeshipOverview")]
    public class MyApprenticeOverviewSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private bool _EmployerConf, _TrainingProviderConf, _ApprenticeshipDetailsConf, _RolesAndResponsibilitiesConf, _HowApprenticeshipWillBeDeliveredConf;
        private DateTime _confirmationDeadline;
        private ChangeOfCircumstanceNotifications _changeOfCircumstanceNotifications;

        public MyApprenticeOverviewSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given("the confirmation deadline is (.*)")]
        public void GivenTheConfirmationDeadlineIs(DateTime confirmBefore)
        {
            _confirmationDeadline = confirmBefore;
        }

        [Given("the time is (.*)")]
        public void GivenTheTimeIs(DateTime now)
        {
            _context.Time.Now = now;
        }

        [Given(@"the apprenticeship has changes to these sections : (.*), (.*), (.*)")]
        public void GivenTheApprenticeshipHasChabgesToTheseSections(bool providerChanged, bool employerChanged, bool apprenticeshipChanged)
        {
            var coc = ChangeOfCircumstanceNotifications.None;
            if (providerChanged) 
                coc |= ChangeOfCircumstanceNotifications.ProviderDetailsChanged;
            if (employerChanged)
                coc |= ChangeOfCircumstanceNotifications.EmployerDetailsChanged;
            if(apprenticeshipChanged)
                coc |= ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged;

            SetCoC(coc);
        }

        private void SetCoC(ChangeOfCircumstanceNotifications coc)
        {
            _changeOfCircumstanceNotifications = coc;
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
                        .WithBodyAsJson(new
                        {
                            _apprenticeshipId.Id,
                            CommitmentStatementId = 21,
                            ConfirmBefore = _confirmationDeadline,
                            EmployerCorrect = _EmployerConf,
                            TrainingProviderCorrect = _TrainingProviderConf,
                            ApprenticeshipDetailsCorrect = _ApprenticeshipDetailsConf,
                            RolesAndResponsibilitiesCorrect = _RolesAndResponsibilitiesConf,
                            HowApprenticeshipDeliveredCorrect = _HowApprenticeshipWillBeDeliveredConf,
                            ChangeOfCircumstanceNotifications = _changeOfCircumstanceNotifications,
                        }));

            _context.OuterApi.MockServer
                .Given(Request.Create()
                    .UsingPatch()
                    .WithPath("/apprentices/*/apprenticeships/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.OK));
        }

        [When("accessing the overview page")]
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

        [Then("the apprenticeship is updated with the time the page was viewed")]
        public void ThenTheApprenticeshipIsUpdatedWithTheTimeThePageWasViewed()
        {
            var request = _context.OuterApi.MockServer.LogEntries
                .Should().Contain(x =>
                    x.RequestMessage.Path == $"/apprentices/{_userContext.ApprenticeId}/apprenticeships/{_apprenticeshipId.Id}" &&
                    x.RequestMessage.Method == "PATCH").Which;

            JArray patch = (JArray)request.RequestMessage.BodyAsJson;
            patch[0]["path"].ToString().Should().BeEquivalentTo("/LastViewed");
            patch[0]["op"].ToString().Should().BeEquivalentTo("replace");
            patch[0]["value"].Value<DateTime>().Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(1));
        }

        [Given("the apprentice has not confirmed every aspect of the apprenciceship")]
        public void GivenTheApprenticeHasNotConfirmedEveryAspectOfTheApprenciceship()
        {
            _EmployerConf =
                _TrainingProviderConf =
                _ApprenticeshipDetailsConf =
                _RolesAndResponsibilitiesConf =
                _HowApprenticeshipWillBeDeliveredConf = false;
        }

        [Then("the apprentice should not see the ready to confirm banner")]
        public void ThenTheApprenticeShouldNotSeeTheReadyToConfirmBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.AllConfirmed.Should().Be(false);
        }

        [Given("the apprentice has confirmed every aspect of the apprenticeship")]
        public void WhenTheApprenticeHasConfirmedEveryAspectOfTheApprenciceship()
        {
            _EmployerConf =
                _TrainingProviderConf =
                _ApprenticeshipDetailsConf =
                _RolesAndResponsibilitiesConf =
                _HowApprenticeshipWillBeDeliveredConf = true;
        }

        [Then("the apprentice should see the ready to confirm banner")]
        public void ThenTheApprenticeShouldSeeTheReadyToConfirmBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.AllConfirmed.Should().Be(true);
        }

        [Then("the apprentice should see (.*) remaining")]
        public void ThenTheApprenticeShouldSeeDaysRemaining(string days)
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.Pluralise(model.DaysRemaining, "day").Should().Be(days);
        }

        [Then("the overdue state should be (.*)")]
        public void ThenTheOverdueStateShouldBe(bool overdue)
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.Overdue.Should().Be(overdue);
        }

        [Then("the apprentice should not see the change notification banner")]
        public void ThenTheApprenticeShouldNotSeeTheChangeNotificationBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.ShowChangeNotification.Should().BeFalse();
        }

        [Then("the apprentice should see the change notification banner")]
        public void ThenTheApprenticeShouldSeeTheChangeNotificationBanner()
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.ShowChangeNotification.Should().BeTrue();
        }

        [Then(@"the message starts like (.*)")]
        public void ThenTheMessageStartsLike(string expectedMessage)
        {
            var model = _context.ActionResult.LastPageResult.Model.As<ConfirmApprenticeshipModel>();
            model.Should().NotBeNull();
            model.ChangeNotificationsMessage.Should().StartWith(expectedMessage);
        }
    }
}