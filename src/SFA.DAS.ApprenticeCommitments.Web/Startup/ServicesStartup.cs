using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;
using SFA.DAS.ApprenticePortal.SharedUi.Services;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services,
            IWebHostEnvironment environment)
        {
            services.AddTransient<ApprenticeApi>();
            services.AddTransient<AuthenticatedUserClient>();
            services.AddTransient<ISimpleUrlHelper, AspNetCoreSimpleUrlHelper>();
            services.AddScoped<ITimeProvider, UtcTimeProvider>();
            services.AddTransient<IMenuVisibility, MenuVisibility>();
            services.AddDomainHelper(environment);

            return services;
        }

        private static IServiceCollection AddDomainHelper(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var domain = ".localhost";
            if (!environment.IsDevelopment())
            {
                domain = ".apprenticeships.education.gov.uk";
            }

            services.AddSingleton(new DomainHelper(domain));
            return services;
        }

        public static IServiceCollection AddOuterApi(
            this IServiceCollection services,
            OuterApiConfiguration configuration)
        {
            services.AddHealthChecks();
            services.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            services.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            services.AddTransient<Http.MessageHandlers.ApimHeadersHandler>();

            services
                .AddRestEaseClient<IOuterApiClient>(configuration.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.ApimHeadersHandler>()
                .AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            services.AddTransient<IApimClientConfiguration>((_) => configuration);

            return services;
        }
    }

    public class OuterApiConfiguration : IApimClientConfiguration
    {
        public string ApiBaseUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
        public string ApiVersion { get; set; } = null!;
    }
}