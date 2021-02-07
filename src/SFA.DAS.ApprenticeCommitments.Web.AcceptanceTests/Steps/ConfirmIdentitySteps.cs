using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "ConfirmIdentity")]
    public class ConfirmIdentitySteps : StepsBase
    {
        private readonly TestContext _context;
        private Guid _registrationId = Guid.NewGuid();

        public ConfirmIdentitySteps(TestContext context) : base(context)
        {
            _context = context;
        }

        [Given("the apprentice has not logged in")]
        public void GivenTheApprenticeHasNotLoggedIn()
        {
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_registrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_registrationId.ToString());
        }

        [Given("the apprentice has not verified his identity")]
        public void GivenTheApprenticeHasNotVerifiedHisIdentity()
        {
            _context.OuterApi.MockServer.Given(
                Request.Create()
                    .UsingGet()
                    .WithUrl(_context.OuterApi.UrlPath($"registrations/{_registrationId}"))
                    )
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(new
                    {
                        Id = _registrationId,
                        EmailAddress = "bob",
                    }));
        }

        [When("first accessing the commitment statement website")]
        public async Task WhenFirstAccessingTheCommitmentStatementWebsite()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "account");
            await _context.Web.Get("ConfirmYourIdentity");
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
    }
}