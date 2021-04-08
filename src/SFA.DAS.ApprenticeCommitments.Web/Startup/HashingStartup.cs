using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class HashingStartup
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            HashingConfiguration configuration)
        {
            services.AddHashingService(configuration);
            services.AddScoped<RequiresIdentityConfirmedMiddleware>();
            return services;
        }

        private static IServiceCollection AddHashingService(
            this IServiceCollection services,
            HashingConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton<IHashingService>(
                new HashingService.HashingService(
                    configuration.AllowedHashstringCharacters,
                    configuration.Hashstring));

            services.AddRouting(options =>
                options.ConstraintMap.Add("HashedId", typeof(HashedIdRouteConstraint)));

            return services;
        }

        public static IApplicationBuilder RequireIdentity(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequiresIdentityConfirmedMiddleware>();
        }
    }

    public class HashingConfiguration
    {
        public string AllowedHashstringCharacters { get; set; }
        public string Hashstring { get; set; }
    }
}