using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Web;
using SFA.DAS.ApprenticeCommitments.Web.Startup;

namespace SFA.DAS.ApprenticeCommitments.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            NLogStartup.ConfigureNLog();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAzureTableConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<ApplicationStartup>();
                })
                .UseNLog();
    }
}