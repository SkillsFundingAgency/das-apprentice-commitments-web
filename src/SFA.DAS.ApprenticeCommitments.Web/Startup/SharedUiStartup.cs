using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Apprentice.SharedUi;
using SFA.DAS.Apprentice.SharedUi.GoogleAnalytics;
using SFA.DAS.Apprentice.SharedUi.Menu;
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

            services.SetZenDeskConfiguration(configuration.Zendesk);
            services.EnableGoogleAnalytics(configuration.GoogleAnalytics);
            services.AddTransient(_ => new NavigationUrlHelper(configuration.ApplicationUrls));

            return services;
        }
    }
}