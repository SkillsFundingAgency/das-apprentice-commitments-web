using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class AuthenticationStartup
    {
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            AuthenticationServiceConfiguration config,
            IWebHostEnvironment environment)
        {
            services
                .AddApplicationAuthentication(config, environment)
                .AddApplicationAuthorisation();

            services.AddTransient((_) => config);

            return services;
        }

        private static IServiceCollection AddApplicationAuthentication(
            this IServiceCollection services,
            AuthenticationServiceConfiguration config,
            IWebHostEnvironment environment)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddApprenticeAuthentication(config.MetadataAddress, environment);

            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();

            return services;
        }
        
        public static void AddGovLoginAuthentication(
            this IServiceCollection services,
            NavigationSectionUrls config,
            IConfiguration configuration)
        {
            services.AddGovLoginAuthentication(configuration);
            services.AddApplicationAuthorisation();
            services.AddTransient<IApprenticeAccountProvider, ApprenticeAccountProvider>();
            services.AddTransient((_) => config);
        }

        private static IServiceCollection AddApplicationAuthorisation(
            this IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddRazorPages(o => o.Conventions
                .AuthorizeFolder("/")
                .AllowAnonymousToPage("/ping")
                .AllowAnonymousToPage("/Accountnew"));
            services.AddControllersWithViews();
            services.AddScoped<AuthenticatedUser>();
            services.AddScoped(s => s
                .GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new());

            return services;
        }
    }

    public class AuthenticationServiceConfiguration
    {
        public string MetadataAddress { get; set; } = null!;
        public string ChangeEmailPath { get; set; } = "/changeemail";
    }
}