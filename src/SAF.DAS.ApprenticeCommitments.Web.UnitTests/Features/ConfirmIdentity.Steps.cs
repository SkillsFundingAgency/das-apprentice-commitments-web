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
    public class ConfirmIdentitySteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private ConfirmYourIdentityModel _postedRegistration;

        public ConfirmIdentitySteps(TestContext context, RegisteredUserContext userContext) : base(context)
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
        }

        [Given("the apprentice has not logged in")]
        public void GivenTheApprenticeHasNotLoggedIn()
        {
        }

        [Given("the apprentice has logged in")]
        [Given("a logged in user")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [Given("an unverified logged in user")]
        public void GivenAVerifiedApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUnverifiedUser(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [Given("the apprentice has not verified their identity")]
        public void GivenTheApprenticeHasNotVerifiedTheirIdentity()
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
                        HasViewedVerification = false
                    }));
        }

        [Given("the apprentice has not verified their identity, but has seen this page")]
        public void GivenTheApprenticeHasNotVerifiedTheirIdentityButHasSeenThjisPage()
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

        [When(@"accessing the ""(.*)"" page")]
        public async Task WhenAccessingThePage(string page)
        {
            await _context.Web.Get(page);
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then("the apprentice should see the verify identity page")]
        public void ThenTheApprenticeShouldSeeTheVerifyIdentityPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<ConfirmYourIdentityModel>().Which.EmailAddress.Should().Be("bob");
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

        [Then(@"the apprentice does not try to mark the registration as seen")]
        public void ThenTheApprenticeDoesNotTryToMarksTheRegistrationAsSeen()
        {
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath($"/registrations/{_userContext.ApprenticeId}/firstseen")
                    .UsingPost()
                                                                               );

            registrationPosts.Should().BeEmpty();
        }

        [Given("the apprentice has verified their identity")]
        public void GivenTheApprenticeHasVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/registrations/{_userContext.ApprenticeId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _userContext.ApprenticeId,
                        Email = "bob",
                        HasCompletedVerification = true,
                    }));
        }

        [When(@"the apprentice should be shown the ""(.*)"" page")]
        public void WhenTheApprenticeShouldBeShownThePage(string page)
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("/apprenticeships");
        }

        [When("the apprentice verifies their identity with")]
        public async Task WhenTheApprenticeVerifiesTheirIdentityWith(Table table)
        {
            _postedRegistration = table.CreateInstance(() => new ConfirmYourIdentityModel(null));
            _postedRegistration.DateOfBirth =
                new DateModel(DateTime.Parse(table.Rows[0]["Date of Birth"]));

            await _context.Web.Post("ConfirmYourIdentity",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "FirstName", _postedRegistration.FirstName },
                    { "LastName", _postedRegistration.LastName },
                    { "DateOfBirth.Day", _postedRegistration?.DateOfBirth?.Day.ToString() },
                    { "DateOfBirth.Month", _postedRegistration?.DateOfBirth?.Month.ToString() },
                    { "DateOfBirth.Year", _postedRegistration?.DateOfBirth?.Year.ToString() },
                }));
        }

        [Then("verification is successful")]
        public void ThenTheVerificationIsSuccessful()
        {
            _context.Web.Response.StatusCode.As<int>().Should().BeLessThan(400);
        }

        [Then("verification is sent to the API")]
        public void ThenTheVerificationIsSuccessfulSent()
        {
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
            Request.Create()
                .WithPath($"/registrations*")
                .UsingPost()
                                                                               );

            registrationPosts.Should().NotBeEmpty();

            var post = registrationPosts.First();

            post.RequestMessage.Path.Should().Be("/registrations");
            var reg = JsonConvert.DeserializeObject<VerifyRegistrationRequest>(post.RequestMessage.Body);
            reg.Should().BeEquivalentTo(new VerifyRegistrationRequest
            {
                ApprenticeId = _userContext.ApprenticeId,
                FirstName = _postedRegistration.FirstName,
                LastName = _postedRegistration.LastName,
                DateOfBirth = _postedRegistration.DateOfBirth.Date,
            });
        }

        [Given("the API will accept the identity")]
        public void WhenTheApiAcceptsTheIdentityAsForInvalidData()
        {
            _context.OuterApi.MockServer
                .Given(Request.Create().WithPath("/registrations*"))
                .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));
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
                .Given(Request.Create().WithPath("/registrations*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithBodyAsJson(errors));
        }

        [Then("verification is not successful")]
        public void ThenTheVerificationIsNotSuccessful()
        {
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<ConfirmYourIdentityModel>()
                .Which.ModelState.IsValid.Should().BeFalse();
        }

        [Then("the apprentice should see the following error messages")]
        public void ThenTheApprenticeShouldSeeTheFollowingErrorMessages(Table table)
        {
            var messages = table.CreateSet<(string PropertyName, string ErrorMessage)>();

            foreach (var (PropertyName, ErrorMessage) in messages)
            {
                _context.ActionResult.LastPageResult
                    .Model.As<ConfirmYourIdentityModel>()
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
                    .Model.As<ConfirmYourIdentityModel>()
                    .ModelState[""]
                    .Errors.Should().ContainEquivalentOf(new { ErrorMessage });
            }
        }
    }
}