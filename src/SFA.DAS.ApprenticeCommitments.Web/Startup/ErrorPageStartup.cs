using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class ErrorPagesStartup
    {
        public static IApplicationBuilder UseErrorPages(
            this IApplicationBuilder app,
            IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseStatusCodePagesWithRedirects("~/error/?statuscode={0}");
            }

            return app;
        }


        public static bool IsDevelopmentOrTest(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment(Environments.Development) ||
            hostEnvironment.IsEnvironment("LOCAL");
        }
    }
}