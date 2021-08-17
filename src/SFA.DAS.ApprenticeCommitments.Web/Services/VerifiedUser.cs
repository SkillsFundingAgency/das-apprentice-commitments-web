using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public static class VerifiedUser
    {
        public const string ClaimName = "VerifiedUser";

        internal static async Task ConfirmIdentity(HttpContext context)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                ((ClaimsIdentity)authenticated.Principal.Identity).AddVerifiedUserClaim();
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }

        internal static ClaimsIdentity CreateVerifiedUserClaim()
            => new ClaimsIdentity(new[] { new Claim(ClaimName, "True") });

        public static void AddVerifiedUserClaim(this ClaimsIdentity identity)
            => identity.AddClaim(new Claim(ClaimName, "True"));

        internal static bool UserHasConfirmedIdentity(HttpContext httpContext)
            => httpContext.User.HasClaim(ClaimName, "True");

        internal static bool UserHasUnconfirmedIdentity(HttpContext httpContext)
            => !UserHasConfirmedIdentity(httpContext);
    }
}