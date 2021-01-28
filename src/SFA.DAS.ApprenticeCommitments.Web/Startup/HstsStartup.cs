using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class HstsStartup
    {
        public static IApplicationBuilder UseHsts(
            this IApplicationBuilder app,
            IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
                app.UseHsts();

            return app;
        }
    }
}