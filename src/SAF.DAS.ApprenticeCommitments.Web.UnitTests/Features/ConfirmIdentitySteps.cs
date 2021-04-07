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
        }

        [Given("the apprentice has not logged in")]
        public void GivenTheApprenticeHasNotLoggedIn()
        {
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.RegistrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.RegistrationId.ToString());
        }

        [Given("the apprentice has not verified their identity")]
        public void GivenTheApprenticeHasNotVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/registrations/{_userContext.RegistrationId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _userContext.RegistrationId,
                        Email = "bob",
                    }));

            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/*/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new[]
                    {
                        new { Id = 1235 },
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

        [Given("the apprentice has verified their identity")]
        public void GivenTheApprenticeHasVerifiedTheirIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/registrations/{_userContext.RegistrationId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _userContext.RegistrationId,
                        Email = "bob",
                        UserId = 12,
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
                    { "NationalInsuranceNumber", _postedRegistration.NationalInsuranceNumber },
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
                RegistrationId = _userContext.RegistrationId,
                FirstName = _postedRegistration.FirstName,
                LastName = _postedRegistration.LastName,
                NationalInsuranceNumber = _postedRegistration.NationalInsuranceNumber,
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