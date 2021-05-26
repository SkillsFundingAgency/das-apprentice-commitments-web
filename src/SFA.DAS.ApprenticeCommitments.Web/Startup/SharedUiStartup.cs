using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprentice.SharedUi.Zendesk;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class SharedUiStartup
    {
        public static IServiceCollection AddSharedUi(
               this IServiceCollection services, ApplicationConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.SetZenDeskConfiguration(configuration.ZenDesk);

            return services;
        }
    }
}
