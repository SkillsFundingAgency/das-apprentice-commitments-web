using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class RegistrationProcessSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private Apprentice _apprentice;
        private readonly string _registrationCode;

        public RegistrationProcessSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
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

        [Given("the apprentice has logged in but not created their account")]
        public void GivenTheApprenticeHasLoggedInButNotCreatedTheirAccount()
        {
            GivenAUserWithoutAccountHasLoggedIn();
            GivenTheApprenticeHasNotCreatedTheirAccount();
        }

        [Given("the apprentice has logged in but not accepted the terms of use")]
        public void GivenTheApprenticeHasLoggedInButNotAcceptedTerms()
        {
            _context.Web.AuthoriseApprenticeWithoutTermsOfUse(_userContext.ApprenticeId);
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new { Apprenticeships = new object[0] { } }));
        }

        [Given("the apprentice has logged in but not matched their account")]
        public void GivenTheApprenticeHasLoggedInButNotMatchedTheirAccount()
        {
            GivenTheApprenticeHasLoggedIn();
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        FirstName = "Bob",
                        LastName = "Bobbertson",
                        DateOfBirth = new DateTime(2000, 01, 13)
                    }));
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new { Apprenticeships = new object[0] { } }));
        }

        [Given("an unverified logged in user")]
        public void GivenAUserWithoutAccountHasLoggedIn()
        {
            TestAuthenticationHandler.AddUserWithoutAccount(_userContext.ApprenticeId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.ApprenticeId.ToString());
        }

        [Given("the apprentice has logged in and matched their account")]
        public void GivenTheApprenticeHasLoggedInAndMatchedTheirAccount()
        {
            GivenTheApprenticeHasLoggedIn();
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*/apprenticeships"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new { Apprenticeships = new[] { new { Id = 1 } } }));
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath("/apprentices/*/apprenticeships/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new { Id = 1 }));
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingAnyMethod()
                    .WithPath("/apprentices/*/apprenticeships/*"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200));
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

        [Then("the apprentice should be redirected to the personal details page")]
        public void ThenTheApprenticeShouldSeeThePersonalDetailsPage()
        {
            _context.Web.Response.Should().Be302Found();
            var redirect = _context.ActionResult.LastRedirectResult;
            redirect.Url.Should().EndWith("//account/Account");
        }

        [Then(@"the apprentice marks the ""(.*)"" registration as seen")]
        public void ThenTheApprenticeMarksTheRegistrationAsSeen(string registrationCode)
        {
            var post = _context.OuterApi.MockServer.LogEntries.Should().Contain(x =>
                x.RequestMessage.Path.Contains($"/registrations/{registrationCode}/firstseen")).Which;

            var reg = JsonConvert.DeserializeObject<RegistrationFirstSeenOnRequest>(post.RequestMessage.Body);
            reg.SeenOn.Should().BeBefore(DateTime.UtcNow);
        }

        [Then("the apprentice does not try to mark the registration as seen")]
        public void ThenTheApprenticeDoesNotTryToMarksTheRegistrationAsSeen()
        {
            var registrationPosts = _context.OuterApi.MockServer.FindLogEntries(
                Request.Create()
                    .WithPath("/registrations/*/firstseen")
                    .UsingPost()
                                                                               );

            registrationPosts.Should().BeEmpty();
        }

        [Given("the apprentice has created their account")]
        public void GivenTheApprenticeHasCreatedTheirAccount()
        {
            _apprentice = new Apprentice
            {
                ApprenticeId = Guid.NewGuid(),
                Email = "someone@example.com",
                FirstName = "Someone",
                LastName = "Wurst",
                DateOfBirth = new DateTime(2008, 08, 21),
                TermsOfUseAccepted = true,
            };

            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithPath($"/apprentices/{_userContext.ApprenticeId}")
                                              )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(_apprentice));
        }

        [When("the apprentice should be shown the Overview page")]
        public async Task WhenTheApprenticeShouldBeShownTheOverviewPage()
        {
            _context.Web.Response
                .Should().Be302Redirect()
                .And.HaveHeader("Location").And.Match("/apprenticeships");
        }

        [Then("the apprentice should be shown the Home page")]
        public void WhenTheApprenticeShouldBeShownTheHomePage()
        {
            ThenTheApprenticeShouldBeShownThePage("https://home/Home");
        }

        [Then("the apprentice should be shown the Home page with a Matched notification")]
        public void ThenTheApprenticeShouldBeShownTheHomePageMatched()
        {
            ThenTheApprenticeShouldBeShownThePage("https://home/Home?notification=ApprenticeshipMatched");
        }

        [Then("the apprentice should be shown the Home page with a Not Matched notification")]
        public void ThenTheApprenticeShouldBeShownThePageWithNotMatched()
        {
            _context.Web.Response.Should().Be2XXSuccessful();
            _context.ActionResult.LastPageResult.Should().NotBeNull();
            _context.ActionResult.LastPageResult.Model.Should().BeOfType<CheckYourDetails>()
                .Which.Should().BeEquivalentTo(new
                {
                    FirstName = "Bob",
                    LastName = "Bobbertson",
                    DateOfBirth = new DateTime(2000, 01, 13),
                });
        }

        [Then(@"the apprentice should be shown the page ""(.*)""")]
        public void ThenTheApprenticeShouldBeShownThePage(string expectedLocation)
        {
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

        [Given("the apprentice has a registration code")]
        public async Task GivenTheApprenticeHasARegistrationCode()
        {
            await _context.Web.Get($"Register/{_registrationCode}");
            _context.Web.Response.Should().Be302Redirect();
        }

        [When("the apprentice accepts the terms of use")]
        public async Task WhenTheApprenticeAcceptsTheTermsOfUse()
        {
            await _context.Web.Post("TermsOfUse",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "ApprenticeId", Guid.NewGuid().ToString() },
                    { "TermsOfUseAccepted", "true" },
                }));

            await _context.Web.FollowLocalRedirects();
        }

        [Then("verification is successful")]
        public void ThenTheVerificationIsSuccessful()
        {
            _context.Web.Response.StatusCode.As<int>().Should().BeLessThan(400);
        }

        [Then("the apprentice account is updated")]
        public void ThenTheAccountIsUpdated()
        {
            var posts = _context.OuterApi.MockServer.FindLogEntries(
            Request.Create()
                .WithPath("/apprentices*")
                .UsingPatch());

            posts.Should().NotBeEmpty();
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

        [Then(@"the registration code should be ""(.*)""")]
        public void ThenTheRegistrationCodeShouldBe(string code)
        {
            var cookie = _context.Web.Cookies.GetCookies(_context.Web.BaseAddress).FirstOrDefault(x=>x.Name == "RegistrationCode");
            cookie.Value.Should().Be(code);
        }
    }
}