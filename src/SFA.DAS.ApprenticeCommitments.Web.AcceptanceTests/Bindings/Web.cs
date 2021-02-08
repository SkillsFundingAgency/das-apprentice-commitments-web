using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks;
using SFA.DAS.ApprenticeCommitments.Web.Startup;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Bindings
{
    [Binding]
    public class Web
    {
        public static HttpClient Client { get; set; }
        public static LocalWebApplicationFactory<ApplicationStartup> Factory { get; set; }
        public Hook<IActionResult>  ActionResultHook;

        private readonly TestContext _context;

        public Web(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            if (Client == null)
            {
                var config = new Dictionary<string, string>
                {
                    {"EnvironmentName", "ACCEPTANCE_TESTS"},
                };

                ActionResultHook = new Hook<IActionResult>();
                Factory = new LocalWebApplicationFactory<ApplicationStartup>(_context, config, ActionResultHook);
                Client = Factory.CreateClient();
            }
            _context.Web = new ApprenticeCommitmentsWeb(Client, ActionResultHook);
        }

        [AfterScenario()]
        public void CleanUpScenario()
        {
            _context?.Web?.Dispose();
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            Client?.Dispose();
            Factory?.Dispose();
            Client = null;
        }
    }
}