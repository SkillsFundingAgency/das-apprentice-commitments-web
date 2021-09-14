using Microsoft.AspNetCore.Http;
using System;
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

        internal static bool UserHasNotCreatedAccount(HttpContext httpContext)
            => !UserHasCreatedAccount(httpContext);
    }

    public static class TermsOfUseAcceptedClaim
    {
        public const string ClaimName = "TermsOfUseAccepted";

        internal static ClaimsIdentity CreateTermsOfUseAcceptedClaim()
            => new ClaimsIdentity(new[] { ClaimInstance });

        public static void AddTermsOfUseAcceptedClaim(this ClaimsIdentity identity)
            => identity.AddClaim(ClaimInstance);

        private static readonly Claim ClaimInstance = new Claim(ClaimName, "True");
    }
}