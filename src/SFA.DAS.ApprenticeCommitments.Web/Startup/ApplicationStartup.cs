using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class ApplicationStartup
    {
        public ApplicationStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _Configuration = configuration;
            _Environment = environment;
        }

        public IConfiguration _Configuration { get; }
        public IWebHostEnvironment _Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddApplicationInsightsTelemetry()
                .AddDataProtection(_Configuration, _Environment)
                .AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseErrorPages(env)
                .UseHsts(env)
                .UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}