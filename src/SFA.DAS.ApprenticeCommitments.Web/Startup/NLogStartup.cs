using NLog.Web;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public static class NLogStartup
    {
        public static void ConfigureNLog()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var configFileName = environment == "Development" ? "nlog.Development.config" : "nlog.config";
                NLogBuilder.ConfigureNLog(configFileName);
            }
            catch
            {
                // Nothing to worry about, fallback to the default NLog configuration
            }
        }
    }
}