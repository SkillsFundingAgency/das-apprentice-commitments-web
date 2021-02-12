using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeCommitments.Web.Api;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            services.AddTransient<RegistrationsService>();
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