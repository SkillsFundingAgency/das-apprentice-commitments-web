using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class RequiresIdentityConfirmedAttribute : Attribute
    {
    }

    public class RequiresIdentityConfirmedMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (ShouldRedirect(context))
                context.Response.Redirect("/ConfirmYourIdentity");
            else
                await next(context);
        }

        private bool ShouldRedirect(HttpContext context)
        {
            if (!RequiresIdentityConfirmed(context)) return false;
            if (context.User.HasClaim("VerifiedUser", "True")) return false;
            return true;
        }

        private static bool RequiresIdentityConfirmed(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<RequiresIdentityConfirmedAttribute>();
            return attribute != null;
        }
    }
}