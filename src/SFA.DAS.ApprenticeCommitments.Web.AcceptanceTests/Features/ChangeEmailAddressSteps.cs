using FluentAssertions;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    public class ChangeEmailAddressSteps
    {
        private readonly TestContext _context;

        public ChangeEmailAddressSteps(TestContext context) => _context = context;

        [Then(@"the result should redirect to the identity server's ""(.*)""")]
        public void ThenTheResultShouldRedirectToTheIdentityServerS(string url)
        {
            _context.Web.Response.RequestMessage.RequestUri.Should().Be($"{_context.IdentityServiceUrl}/changeemail");
        }
    }
}
