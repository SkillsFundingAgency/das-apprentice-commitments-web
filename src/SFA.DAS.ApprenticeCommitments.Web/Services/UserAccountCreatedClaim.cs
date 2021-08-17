using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public static class UserAccountCreatedClaim
    {
        public const string ClaimName = "AccountCreated";

        internal static async Task UserAccountCreated(this HttpContext context)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                ((ClaimsIdentity)authenticated.Principal.Identity).AddAccountCreatedClaim();
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }

        internal static ClaimsIdentity CreateAccountCreatedClaim()
            => new ClaimsIdentity(new[] { new Claim(ClaimName, "True") });

        public static void AddAccountCreatedClaim(this ClaimsIdentity identity)
            => identity.AddClaim(new Claim(ClaimName, "True"));

        internal static bool UserHasCreatedAccount(HttpContext httpContext)
            => httpContext.User.HasClaim(ClaimName, "True");

        internal static bool UserHasNotCreatedAccount(HttpContext httpContext)
            => !UserHasCreatedAccount(httpContext);
    }
}