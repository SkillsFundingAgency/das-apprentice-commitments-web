using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "ConfirmIdentity")]
    public class ConfirmIdentitySteps : StepsBase
    {
        private readonly TestContext _context;

        public ConfirmIdentitySteps(TestContext context) : base(context)
        {
            _context = context;
        }

        [Given(@"the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
        }

        [Given(@"the apprentice has not verified his identity")]
        public void GivenTheApprenticeHasNotVerifiedHisIdentity()
        {
        }

        [When(@"first accessing the commitment statement website")]
        public async Task WhenFirstAccessingTheCommitmentStatementWebsite()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "account");
           await _context.Web.Get("account");

        }

        [Then(@"the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Then(@"the apprentice should see the verify identity page")]
        public void ThenTheApprenticeShouldSeeTheVerifyIdentityPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().NotBeNull();
            page.ViewData.Count.Should().Be(1);
            page.ViewData["Title"].As<string>().Should().Be("Confirm your identity");
        }

    }
}
