using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public static class VerifiedUser
    {
        internal static async Task ConfirmIdentity(HttpContext context)
        {
            var identity = (ClaimsIdentity)context.User.Identity;
            identity.AddClaim(new Claim("VerifiedUser", "True"));
            await context.SignInAsync(context.User);
        }

        internal static bool UserHasConfirmedIdentity(HttpContext httpContext)
            => httpContext.User.HasClaim("VerifiedUser", "True");

        internal static bool UserHasUnconfirmedIdentity(HttpContext httpContext)
            => !UserHasConfirmedIdentity(httpContext);
    }
}