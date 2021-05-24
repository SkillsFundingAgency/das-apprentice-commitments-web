using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ApprenticeCommitments.Web.Startup;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;
        private readonly IHook<IActionResult> _actionResultHook;
        private readonly TestContext _testContext;

        public LocalWebApplicationFactory(TestContext testContext, Dictionary<string, string> config, IHook<IActionResult> actionResultHook)
        {
            _config = config;
            _actionResultHook = actionResultHook;
            _testContext = testContext;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x => x.UseStartup<TEntryPoint>());
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication("TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestScheme", _ => { });
            });

            builder.ConfigureServices(s =>
            {
                s.AddControllersWithViews(options =>
                {
                    options.Filters.Add(new ActionResultFilter(_actionResultHook));
                });

                s.AddMvc().AddRazorPagesOptions(o =>
                {
                    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                });
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment("LOCAL");
        }
    }
}