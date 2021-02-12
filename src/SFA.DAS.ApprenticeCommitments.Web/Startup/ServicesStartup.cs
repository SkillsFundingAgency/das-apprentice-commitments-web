using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeCommitments.Web.Api;
using SFA.DAS.ApprenticeCommitments.Web.Pages;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<RegistrationsService>();
            services.AddScoped<RegistrationUser>();
            services.AddScoped(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext.User);

            var outerApiConfig = services.BuildServiceProvider().GetRequiredService<OuterApiConfig>();
            var url = outerApiConfig.BaseUrl;
            services.AddRestEaseClient<IApiClient>(url);

            return services;
        }
    }

    public class OuterApiConfig
    {
        public string BaseUrl { get; set; }
    }
}