using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class HttpSteps : StepsBase
    {
        private readonly TestContext _context;

        public HttpSteps(TestContext context) : base(context)
            => _context = context;

        [When(@"accessing the ""(.*)"" page")]
        public async Task WhenAccessingThePage(string page)
        {
            await _context.Web.Get(page);
            await _context.Web.FollowLocalRedirects();
        }

        [Then("the response status code should be Ok")]
        public void ThenTheResponseStatusCodeShouldBeOk()
            => _context.Web.Response.Should().Be200Ok();
    }
}