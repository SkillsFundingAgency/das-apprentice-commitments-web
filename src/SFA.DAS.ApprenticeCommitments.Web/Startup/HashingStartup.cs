using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class HashingStartup
    {
        public static IServiceCollection AddHashingService(
            this IServiceCollection serviceCollection,
            HashingConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            serviceCollection.AddSingleton<IHashingService>(
                new HashingService.HashingService(
                    configuration.AllowedHashstringCharacters,
                    configuration.Hashstring));

            return serviceCollection;
        }
    }

    public class HashingConfiguration
    {
        public virtual string AllowedHashstringCharacters { get; set; }
        public virtual string Hashstring { get; set; }
    }
}