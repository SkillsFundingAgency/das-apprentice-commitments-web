using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public static class UserAccountCreatedClaim
    {
        public const string ClaimName = "AccountCreated";

        internal static ClaimsIdentity CreateAccountCreatedClaim()
            => new ClaimsIdentity(new[] { new Claim(ClaimName, "True") });

        public static void AddAccountCreatedClaim(this ClaimsIdentity identity)
            => identity.AddClaim(new Claim(ClaimName, "True"));

        internal static bool UserHasCreatedAccount(HttpContext httpContext)
            => httpContext.User.HasClaim(ClaimName, "True");

        internal static bool UserMustCreateAccount(this HttpContext httpContext)
            => !UserHasCreatedAccount(httpContext);
    }
}