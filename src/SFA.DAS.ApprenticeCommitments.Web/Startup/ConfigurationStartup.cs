using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ApprenticeCommitments.Web.Extension;
using SFA.DAS.Configuration.AzureTableStorage;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ConfigurationStartup
    {
        public static IWebHostBuilder ConfigureAzureTableConfiguration(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.Build();

                if (!config["EnvironmentName"]!.Equals("DEV", StringComparison.InvariantCultureIgnoreCase))
                {
                    configBuilder.AddAzureTableStorage(options =>
                    {
                        var (names, connectionString, environment) = configBuilder.EmployerConfiguration();
                        options.ConfigurationKeys = names.Split(",");
                        options.StorageConnectionString = connectionString;
                        options.EnvironmentName = environment;
                        options.PreFixConfigurationKeys = false;
                    });
                }

                if (!config.IsAcceptanceTest())
                {
                    configBuilder.AddJsonFile("appsettings.development.json", true);
                }
            });
            return hostBuilder;
        }

        private static (string names, string connectionString, string environment)
            EmployerConfiguration(this IConfigurationBuilder configBuilder)
        {
            var config = configBuilder.Build();
            return
                (
                    config["ConfigNames"]!,
                    config["ConfigurationStorageConnectionString"]!,
                    config["EnvironmentName"]!
                );
        }
    }
}