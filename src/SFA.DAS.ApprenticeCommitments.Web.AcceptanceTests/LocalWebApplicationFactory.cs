using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests
{
    public class LocalWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly Dictionary<string, string> _config;
        private readonly IHook<IActionResult> _actionResultHook;


        public LocalWebApplicationFactory(Dictionary<string, string> config, IHook<IActionResult> actionResultHook)
        {
            _config = config;
            _actionResultHook = actionResultHook;
        }


        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(x =>
                {
                    x.UseStartup<TEntryPoint>();
                });
            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddControllersWithViews(options =>
                {
                    options.Filters.Add(new ActionResultFilter(_actionResultHook));
                });
            });

            builder.ConfigureAppConfiguration(a =>
            {
                a.Sources.Clear();
                a.AddInMemoryCollection(_config);
            });
            builder.UseEnvironment("LOCAL");
        }
    }
}