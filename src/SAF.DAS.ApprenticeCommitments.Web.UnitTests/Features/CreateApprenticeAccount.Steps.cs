using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class CreateApprenticeAccountSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private AccountModel _postedRegistration;
        private string _registrationCode;

        public CreateApprenticeAccountSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;

            _context.OuterApi?.MockServer.Given(
                     Request.Create()
                         .UsingPost()
                         .WithPath($"/registrations/{_userContext.ApprenticeId}/firstseen")
                                               )
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.Accepted));

            _registrationCode = Guid.NewGuid().ToString();
        }

        [Given("the apprentice has not logged in")]
        public void GivenTheApprenticeHasNotLoggedIn()
        {
        }

        [Given("the apprentice has logged in")]
        [Given("a logged in user")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            _context.Web.AuthoriseApprentice(_userContext.ApprenticeId);
        }

        [Given("an unverified logged in user")]
        public void GivenAVerifiedApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUnverifiedUser(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [Given("the apprentice has not created their account")]
        public void GivenTheApprenticeHasNotCreatedTheirAccount()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}"))

                .RespondWith(Response.Create()
                    .WithStatusCode(404));
        }

        [Given("the apprentice has not created their account, but has seen this page")]
        public void GivenTheApprenticeHasNotCreatedTheirAccountButHasSeenThisPage()
        {
            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingGet()
                         .WithPath($"/registrations/{_userContext.ApprenticeId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new VerifyRegistrationResponse()
                    {
                        ApprenticeId = _userContext.ApprenticeId,
                        Email = "bob",
                        HasCompletedVerification = false,
                        HasViewedVerification = true
                    }));
        }

        [Then("the apprentice should see the personal details page")]
        public void ThenTheApprenticeShouldSeeThePersonalDetailsPage()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            var page = _context.ActionResult.LastPageResult;
            page.Should().NotBeNull();
            page.Model.Should().BeOfType<AccountModel>();
        }

        [Then(@"the apprentice marks the registration as seen")]
        public void ThenTheApprenticeMarksTheRegistrationAsSeen()
        {
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/registrations/{_userContext.ApprenticeId}/firstseen")
                    .UsingPost()
                                                                               );

            registrationPosts.Should().NotBeEmpty();
            var post = registrationPosts.First();
            var reg = JsonConvert.DeserializeObject<RegistrationFirstSeenOnRequest>(post.RequestMessage.Body);
            reg.SeenOn.Should().BeBefore(DateTime.UtcNow);
        }

        [Then("the apprentice does not try to mark the registration as seen")]
        public void ThenTheApprenticeDoesNotTryToMarksTheRegistrationAsSeen()
        {
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/registrations/{_userContext.ApprenticeId}/firstseen")
                    .UsingPost()
                                                                               );

            registrationPosts.Should().BeEmpty();
        }

        [Given("the apprentice has created their account")]
        public void GivenTheApprenticeHasCreatedTheirAccount()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _userContext.ApprenticeId,
                        Email = "bob@example.com",
                    }));
        }

        [When("the apprentice should be shown the Overview page")]
        public async Task WhenTheApprenticeShouldBeShownTheOverviewPage()
        {
            _context.Web.Response
                .Should().Be302Redirect()
                .And.HaveHeader("Location").And.Match("/apprenticeships");
        }

        [Then("the apprentice should be shown the Home page")]
        public async Task WhenTheApprenticeShouldBeShownThePage()
        {
            const string expectedLocation = "https://home/?notification=ApprenticeshipMatched";
            _context.Web.Response.Should().Be302Redirect().And.HaveHeader("Location");
            _context.Web.Response.Headers.Location.ToString().Should().Be(expectedLocation);
        }

        [Then("the apprentice is matched to the apprenticeship")]
        public void ThenTheApprenticeIsMatchedToTheApprenticeship()
        {
            var posts = _context.OuterApi.MockServer.FindLogEntries(
                        Request.Create()
                            .WithPath("/apprenticeships*")
                            .UsingPost());

            posts.Should().NotBeEmpty();

            var post = posts.First();

            post.RequestMessage.Path.Should().Be("/apprenticeships");
            var reg = JsonConvert.DeserializeObject<ApprenticeshipAssociation>(post.RequestMessage.Body);
            reg.Should().BeEquivalentTo(new
            {
                _userContext.ApprenticeId,
                RegistrationId = _registrationCode,
            });
        }

        [Then("the apprentice should be shown the Home page with a Not Matched notification")]
        public async Task WhenTheApprenticeShouldBeShownThePageWithNotMatched()
        {
            const string expectedLocation = "https://home/?notification=ApprenticeshipDidNotMatch";
            _context.Web.Response.Should().Be302Redirect().And.HaveHeader("Location");
            _context.Web.Response.Headers.Location.ToString().Should().Be(expectedLocation);
        }

        [When("the apprentice creates their account with")]
        public async Task WhenTheApprenticeCreatesTheirAccountWith(Table table)
        {
            _postedRegistration = table.CreateInstance(() => new AccountModel(null, null));
            _postedRegistration.DateOfBirth =
                new DateModel(DateTime.Parse(table.Rows[0]["Date of Birth"]));

            var response = await _context.Web.Post($"Account?handler=Register&registrationCode={_registrationCode}",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "FirstName", _postedRegistration.FirstName },
                    { "LastName", _postedRegistration.LastName },
                    { "DateOfBirth.Day", _postedRegistration?.DateOfBirth?.Day.ToString() },
                    { "DateOfBirth.Month", _postedRegistration?.DateOfBirth?.Month.ToString() },
                    { "DateOfBirth.Year", _postedRegistration?.DateOfBirth?.Year.ToString() },
                    { "EmailAddress", _postedRegistration?.EmailAddress },
                }));

            await _context.Web.FollowLocalRedirects();
        }

        [Then("verification is successful")]
        public void ThenTheVerificationIsSuccessful()
        {
            _context.Web.Response.StatusCode.As<int>().Should().BeLessThan(400);
        }

        [Then("the apprentice account is updated")]
        public void ThenTheVerificationIsSuccessfulSent()
        {
            var posts = _context.OuterApi.MockServer.FindLogEntries(
            Request.Create()
                .WithPath("/apprentices*")
                .UsingPost());

            posts.Should().NotBeEmpty();

            var post = posts.First();

            post.RequestMessage.Path.Should().Be("/apprentices");
            var reg = JsonConvert.DeserializeObject<Apprentice>(post.RequestMessage.Body);
            reg.Should().BeEquivalentTo(new
            {
                _userContext.ApprenticeId,
                _postedRegistration.FirstName,
                _postedRegistration.LastName,
                DateOfBirth = _postedRegistration.DateOfBirth.Date,
            });
        }

        [Given("the API will accept the account")]
        public void WhenTheApiAcceptsTheAccount()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/apprentices"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [Given("the API will match the apprenticeship")]
        public void WhenTheApiAcceptsTheMatch()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
        }

        [Given("the API will not match the apprenticeship")]
        public void WhenTheApiAcceptsTheMatchNot()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingPost()
                    .WithPath("/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(400));
        }

        [Given("the API will reject the identity with the following errors")]
        public void WhenTheApiRejectsTheIdentity(Table table)
        {
            var errors = table.Rows.Select(row => new ErrorItem
            {
                PropertyName = string.IsNullOrWhiteSpace(row["Property Name"]) ? null : row["Property Name"],
                ErrorMessage = row["Error Message"],
            });

            _context.OuterApi.MockServer
                .Given(Request.Create().WithPath("/apprentices"))
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithBodyAsJson(errors));
        }

        [Then("verification is not successful")]
        public void ThenTheVerificationIsNotSuccessful()
        {
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<AccountModel>()
                .Which.ModelState.IsValid.Should().BeFalse();
        }

        [Then("the apprentice should see the following error messages")]
        public void ThenTheApprenticeShouldSeeTheFollowingErrorMessages(Table table)
        {
            var messages = table.CreateSet<(string PropertyName, string ErrorMessage)>();

            foreach (var (PropertyName, ErrorMessage) in messages)
            {
                _context.ActionResult.LastPageResult
                    .Model.As<AccountModel>()
                    .ModelState[PropertyName]
                    .Errors.Should().ContainEquivalentOf(new { ErrorMessage });
            }
        }

        [Then("the apprentice should see the following extra error messages")]
        public void ThenTheApprenticeShouldSeeTheFollowingExtraErrorMessages(Table table)
        {
            var messages = table.Rows.Select(x => x[0]);

            foreach (var ErrorMessage in messages)
            {
                _context.ActionResult.LastPageResult
                    .Model.As<AccountModel>()
                    .ModelState[""]
                    .Errors.Should().ContainEquivalentOf(new { ErrorMessage });
            }
        }

        [Then(@"the registration code should be ""(.*)""")]
        public void ThenTheRegistrationCodeShouldBe(string code)
        {
            _context.ActionResult.LastPageResult
                .Model.As<AccountModel>()
                .RegistrationCode.Should().Be(code);
        }
    }
}