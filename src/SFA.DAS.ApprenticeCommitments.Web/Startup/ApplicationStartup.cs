using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticePortal.SharedUi.Startup;
using System.Net;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class ApplicationStartup
    {
        public ApplicationStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appConfig = Configuration.Get<ApplicationConfiguration>();

            services
                .AddApplicationInsightsTelemetry()
                .AddDataProtection(appConfig.ConnectionStrings, Environment)
                .AddAuthentication(appConfig.Authentication, Environment)
                .AddOuterApi(appConfig.ApprenticeCommitmentsApi)
                .AddHashingService(appConfig.Hashing)
                .RegisterServices()
                .AddControllers();

            services.AddSharedUi(appConfig, options =>
            {
                options.EnableZendesk();
                options.EnableGoogleAnalytics();
                options.SetCurrentNavigationSection(NavigationSection.ConfirmMyApprenticeship);
            });

            services.AddRazorPages();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRewriter(new RewriteOptions()
                .AddRedirect("ConfirmYourIdentity", "ConfirmYourPersonalDetails", (int)HttpStatusCode.MovedPermanently)
                .AddRedirect("confirmyouridentity", "ConfirmYourPersonalDetails", (int)HttpStatusCode.MovedPermanently)
                );

            app.UseErrorPages(env)
                .UseStatusCodePagesWithReExecute("/Error")
                .UseHsts(env)
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseHealthChecks()
                .UseRouting()
                .UseMiddleware<SecurityHeadersMiddleware>()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                });
        }
    }
}