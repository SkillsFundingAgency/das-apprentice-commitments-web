using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticePortal.SharedUi.Startup;
using System.Net;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.Encoding;
using SFA.DAS.GovUK.Auth.Services;

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
            var encodingConfig = Configuration.Get<EncodingConfig>();

            services
                .AddTelemetryRegistration(Configuration)
                .AddApplicationInsightsTelemetry();

            services
                .AddDataProtection(appConfig!.ConnectionStrings, Environment)
                .AddOuterApi(appConfig!.ApprenticeCommitmentsApi)
                .AddSingleton<IEncodingService>(new EncodingService(encodingConfig))
                .RegisterServices(Environment)
                .AddControllers();
            
            services.AddTransient<ICustomClaims, ApprenticeAccountPostAuthenticationClaimsHandler>();
            if (appConfig.UseGovSignIn)
            {
                services.AddGovLoginAuthentication(appConfig.ApplicationUrls,Configuration);
            }
            else
            {
                services.AddAuthentication(appConfig!.Authentication, Environment);    
            }
            
            
            services.AddSharedUi(appConfig, options =>
            {
                options.EnableZendesk();
                options.EnableGoogleAnalytics();
                options.SetCurrentNavigationSection(NavigationSection.ConfirmMyApprenticeship);
                options.SetUseGovSignIn(appConfig.UseGovSignIn);
            });

            services.AddRazorPages();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRewriter(new RewriteOptions()
                .AddRedirect("ConfirmYourIdentity", "Account", (int)HttpStatusCode.MovedPermanently)
                .AddRedirect("confirmyouridentity", "Account", (int)HttpStatusCode.MovedPermanently)
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