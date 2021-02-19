using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class HealthCheckStartup
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/ping");
            return app;
        }
    }
}