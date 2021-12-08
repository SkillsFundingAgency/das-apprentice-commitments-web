using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class ChangeEmailAddressSteps
    {
        private readonly TestContext _context;
        private string _link;
        private Guid _clientId = Guid.NewGuid();
        private const string Email = "email";
        private const string Token = "token";

        public ChangeEmailAddressSteps(TestContext context)
        {
            _context = context;
        }

        [Then(@"the result should redirect to the identity server's change email page")]
        public void ThenTheResultShouldRedirectToTheIdentityServerS()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("https://identity/changeemail");
        }

        [Given(@"they have received the link to change their email address")]
        public void GivenTheyHaveReceivedTheLinkToChangeTheirEmailAddress()
        {
            _link = $"/profile/{_clientId}/changeemail/confirm?email={Email}&token={Token}";
        }

        [When(@"they click on this link")]
        public async Task WhenTheyClickOnThisLink()
        {
            await _context.Web.Get(_link);
        }

        [Then(@"they should be redirected to the login service confirm new email page")]
        public void ThenTheyShouldBeRedirectedToTheLoginServiceConfirmPage()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be($"https://identity/profile/{_clientId}/changeemail/confirm?email={Email}&token={Token}");
        }
    }
}