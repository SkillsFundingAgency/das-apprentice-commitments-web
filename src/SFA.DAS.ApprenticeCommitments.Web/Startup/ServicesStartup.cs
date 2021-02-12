using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestEase.HttpClientFactory;
using SFA.DAS.ApprenticeCommitments.Web.Api;
using SFA.DAS.ApprenticeCommitments.Web.Pages;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ServicesStartup
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services,
            IWebHostEnvironment environment)
        {
            services.AddTransient<RegistrationsService>();
            services.AddScoped<AuthenticatedUser>();
            services.AddScoped(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext.User);


            if(environment.IsDevelopment())
            {
                services.AddScoped(_ => AuthenticatedUser.FakeUser);
                //services.AddScoped(_ => AuthenticatedUser.FakeUserClaim);
            }

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