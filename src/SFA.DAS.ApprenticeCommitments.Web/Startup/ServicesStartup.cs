using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeCommitments.Web.Api;
using SFA.DAS.ApprenticeCommitments.Web.Pages;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            services.AddTransient<RegistrationsService>();
            services.AddScoped<RegistrationUser>();
            services.AddScoped(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext.User);
            return services;
        }

        public static IServiceCollection AddOuterApi(
            this IServiceCollection services,
            OuterApiConfiguration configuration)
        {
            services.AddTransient<InvalidContentHandler>();

            services
                .AddRestEaseClient<IApiClient>(configuration.BaseUrl)
                .AddHttpMessageHandler<InvalidContentHandler>();
            return services;
        }
    }

    public class OuterApiConfiguration
    {
        public string BaseUrl { get; set; }
    }
}