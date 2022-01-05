using FluentAssertions;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    public class HealthCheckSteps : StepsBase
    {
        private readonly TestContext _context;

        public HealthCheckSteps(TestContext context) : base(context) => _context = context;

        [Then(@"the result should be ""(.*)""")]
        public void ThenTheResultShouldBe(string expected)
        {
            _context.Web.Response.EnsureSuccessStatusCode();
            _context.Web.Response.Content.ReadAsStringAsync().Result.Should().Be(expected);
        }
    }
}