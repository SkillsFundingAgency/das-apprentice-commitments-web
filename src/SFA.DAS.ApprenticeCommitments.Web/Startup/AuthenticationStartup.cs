using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System.IdentityModel.Tokens.Jwt;

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
                .AddApplicationAuthentication(config)
                .AddApplicationAuthorisation();

            services.AddTransient((_) => config);

            return services;
        }

        private static IServiceCollection AddApplicationAuthentication(
            this IServiceCollection services,
            AuthenticationServiceConfiguration config)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            IdentityModelEventSource.ShowPII = true;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = "Cookies";
                    options.Authority = config.MetadataAddress;
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "apprentice";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    options.SaveTokens = true;
                    options.DisableTelemetry = false;

                    options.EventsType = typeof(AuthenticationEvents);
                });

            services.AddScoped<AuthenticationEvents>();

            return services;
        }

        private static IServiceCollection AddApplicationAuthorisation(
            this IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddRazorPages(o => o.Conventions
                .AuthorizeFolder("/")
                .AllowAnonymousToPage("/ping"));
            services.AddScoped<AuthenticatedUser>();
            services.AddScoped(s => s
                .GetRequiredService<IHttpContextAccessor>().HttpContext.User);

            return services;
        }
    }

    public class AuthenticationServiceConfiguration
    {
        public string MetadataAddress { get; set; }
        public string ChangeEmailPath { get; set; } = "/changeemail";
    }
}