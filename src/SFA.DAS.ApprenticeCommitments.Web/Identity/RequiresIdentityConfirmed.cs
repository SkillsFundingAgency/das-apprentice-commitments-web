using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class RequiresIdentityConfirmedAttribute : Attribute
    {
    }

    public class RequiresIdentityConfirmedMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (RestEase.ApiException e) when (ShouldRedirect(e, context))
            {
                context.Response.Redirect("/ConfirmYourIdentity");
            }
        }

        private static bool ShouldRedirect(RestEase.ApiException e, HttpContext context)
            => ShouldCheckIdentityConfirmation(context) && IsUnverifiedAccount(e);

        private static bool ShouldCheckIdentityConfirmation(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<RequiresIdentityConfirmedAttribute>();
            return attribute != null;
        }

        private static bool IsUnverifiedAccount(RestEase.ApiException e)
            => e.StatusCode == System.Net.HttpStatusCode.NotFound;
    }
}