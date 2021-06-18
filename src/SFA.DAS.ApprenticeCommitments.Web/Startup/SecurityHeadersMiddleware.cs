using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Startup
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate next;

        public SecurityHeadersMiddleware(RequestDelegate next) => this.next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.AddIfNotPresent("x-frame-options", new StringValues("DENY"));
            context.Response.Headers.AddIfNotPresent("x-content-type-options", new StringValues("nosniff"));
            context.Response.Headers.AddIfNotPresent("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
            context.Response.Headers.AddIfNotPresent("x-xss-protection", new StringValues("0"));
            context.Response.Headers.AddIfNotPresent(
                "Content-Security-Policy",
                new StringValues(
                    "default-src 'self' das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net " +
                    "das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net " +
                    "'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com https://*.rcrsv.io ;"));

            await next(context);
        }
    }
}