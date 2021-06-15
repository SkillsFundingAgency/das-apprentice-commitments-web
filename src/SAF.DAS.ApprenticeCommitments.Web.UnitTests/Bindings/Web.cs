using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Startup;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using SFA.DAS.HashingService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Bindings
{
    [Binding]
    public class Web
    {
        private Fixture _fixture = new Fixture();
        public static HttpClient Client { get; set; }
        public static Dictionary<string, string> Config { get; private set; }
        public static LocalWebApplicationFactory<ApplicationStartup> Factory { get; set; }

        public static Hook<IActionResult> ActionResultHook;

        private readonly TestContext _context;
        private static readonly Func<SpecifiedTimeProvider> _time = () => _timeProvider;
        private static SpecifiedTimeProvider _timeProvider;

        public Web(TestContext context)
        {
            _context = context;
            _timeProvider = _context.Time;
        }

        [BeforeScenario()]
        public void Initialise()
        {
            if (Client == null)
            {
                Config = new Dictionary<string, string>
                {
                    {"EnvironmentName", "ACCEPTANCE_TESTS"},
                    {"Authentication:MetadataAddress", _context.IdentityServiceUrl},
                    {"ApprenticeCommitmentsApi:ApiBaseUrl", _context.OuterApi?.BaseAddress ?? "https://api/"},
                    {"ApprenticeCommitmentsApi:SubscriptionKey", ""},
                    {"Hashing:AllowedHashstringCharacters", "abcdefgh12345678"},
                    {"Hashing:Hashstring", "testing"},
                    {"ZenDesk:ZendeskSectionId", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskSnippetKey", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskCobrowsingSnippetKey", _fixture.Create<string>()},
                };

                ActionResultHook = new Hook<IActionResult>();
                Factory = new LocalWebApplicationFactory<ApplicationStartup>(Config, ActionResultHook, _time);
                Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });
            }
            _context.Web = new ApprenticeCommitmentsWeb(Client, ActionResultHook, Config);
            _context.Hashing = Factory.Services.GetRequiredService<IHashingService>();
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