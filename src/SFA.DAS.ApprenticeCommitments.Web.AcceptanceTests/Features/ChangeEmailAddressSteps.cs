using FluentAssertions;
using System.Net;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    public class ChangeEmailAddressSteps
    {
        private readonly TestContext _context;

        public ChangeEmailAddressSteps(TestContext context) => _context = context;

        [Then(@"the result should redirect to the identity server's change email page")]
        public void ThenTheResultShouldRedirectToTheIdentityServerS()
        {
            _context.Web.Response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            _context.Web.Response.Headers.Location.Should().Be("https://identity/changeemail");
        }
    }
}