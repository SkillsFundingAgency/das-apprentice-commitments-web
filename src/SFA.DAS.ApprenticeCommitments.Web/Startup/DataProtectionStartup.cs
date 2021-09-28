using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Azure.Extensions.AspNetCore.DataProtection.Blobs;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class DataProtectionStartup
    {
        public static IServiceCollection AddDataProtection(
            this IServiceCollection services,
            DataProtectionConnectionStrings configuration,
            IWebHostEnvironment environment)
        {
            //if (!environment.IsDevelopment() && configuration != null)
            //{
            //    var redisConnectionString = configuration.RedisConnectionString;
            //    var dataProtectionKeysDatabase = configuration.DataProtectionKeysDatabase;

            //    var redis = ConnectionMultiplexer
            //        .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

            //    services.AddDataProtection()
            //        .SetApplicationName("apprentice-commitments")
            //        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            //}

            // This is released for a test only
            var conx = "DefaultEndpointsProtocol=https;AccountName=comtstore01;AccountKey=Ie4vbuvys1aplZjaKUiIjdCDfiXfP+a37ctdnQMiL1HBy97Vzd9PNmWdMagrR4cOPD5DeeSG0uZVuj1G4mKJcA==;EndpointSuffix=core.windows.net";
            var container = "cont01";
            var blob = "keyblob";

            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(conx, container, blob)
                .SetApplicationName("apprentice-commitments");

            return services;
        }
    }

    public class DataProtectionConnectionStrings
    {
        public string RedisConnectionString { get; set; } = null!;
        public string DataProtectionKeysDatabase { get; set; } = null!;
    }
}