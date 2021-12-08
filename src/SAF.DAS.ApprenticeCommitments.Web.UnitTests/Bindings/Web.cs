using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Startup;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using SFA.DAS.HashingService;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Bindings
{
    [Binding]
    public class Web
    {
        private Fixture _fixture = new Fixture();
        public static HttpClient Client { get; set; }
        public static CookieContainer Cookies { get; set; }
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
                    {"ApplicationUrls:ApprenticeHomeUrl", "https://home/"},
                    {"ApplicationUrls:ApprenticeAccountsUrl", "https://account/"},
                    {"ApplicationUrls:ApprenticeCommitmentsUrl", "https://confirm/"},
                    {"ApplicationUrls:ApprenticeLoginUrl", "https://login/"},
                    {"ApprenticeCommitmentsApi:SubscriptionKey", ""},
                    {"Hashing:AllowedHashstringCharacters", "abcdefgh12345678"},
                    {"Hashing:Hashstring", "testing"},
                    {"ZenDesk:ZendeskSectionId", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskSnippetKey", _fixture.Create<string>()},
                    {"ZenDesk:ZendeskCobrowsingSnippetKey", _fixture.Create<string>()},
                };

                ActionResultHook = new Hook<IActionResult>();
                Factory = new LocalWebApplicationFactory<ApplicationStartup>(Config, ActionResultHook, _time);
                var handler = new CookieContainerHandler()
                {
                    InnerHandler = Factory.Server.CreateHandler(),
                };
                Client = new HttpClient(handler) { BaseAddress = Factory.Server.BaseAddress };
                Cookies = handler.Container;
            }

            _context.Web = new ApprenticeCommitmentsWeb(Client, ActionResultHook, Config, Cookies);
            _context.Hashing = Factory.Services.GetRequiredService<IHashingService>();
            TestAuthenticationHandler.Authentications.Clear();
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