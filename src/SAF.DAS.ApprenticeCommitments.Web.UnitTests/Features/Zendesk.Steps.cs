using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TestContext = SFA.DAS.ApprenticeCommitments.Web.UnitTests.TestContext;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "Zendesk")]
    public class ZendeskSteps : StepsBase
    {
        private readonly TestContext _context;
        private Guid apprenticeId = Guid.NewGuid();

        public ZendeskSteps(TestContext context) : base(context)
        {
            _context = context;
            context.Web.AuthoriseApprentice(apprenticeId);
        }

        [When("a page is requested")]
        public async Task WhenPageIsRequested()
        {
            await _context.Web.Get("Apprenticeships/abcd123/ConfirmYourEmployer");
        }

        [Then("the page contains the Zendesk configuration")]
        public async Task ThenThePageContainsTheZendeskConfiguration()
        {
            Assert.That(_context.Web.Response, Is.Not.Null);
            var body = await _context.Web.Response.Content.ReadAsStringAsync();
            Assert.That(body, Contains.Substring($"section: '{_context.Web.Config["ZenDesk:ZendeskSectionId"]}'"));
            Assert.That(body, Contains.Substring(
                $@"<script id=""ze-snippet"" src=""https://static.zdassets.com/ekr/snippet.js?key={_context.Web.Config["ZenDesk:ZendeskSnippetKey"]}"""));
            Assert.That(body, Contains.Substring(
                $@"<script id=""co-snippet"" src=""https://embed-euw1.rcrsv.io/{_context.Web.Config["ZenDesk:ZendeskCobrowsingSnippetKey"]}?zwwi=1"""));
        }
    }
}