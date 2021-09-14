using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public static class TermsOfUseAcceptedClaim
    {
        public const string ClaimName = "TermsOfUseAccepted";

        internal static ClaimsIdentity CreateTermsOfUseAcceptedClaim()
            => new ClaimsIdentity(new[] { ClaimInstance });

        public static void AddTermsOfUseAcceptedClaim(this ClaimsIdentity identity)
            => identity.AddClaim(ClaimInstance);

        internal static bool UserMustAcceptTermsOfUse(this HttpContext httpContext)
            => !httpContext.User.HasClaim(ClaimName, "True");

        private static readonly Claim ClaimInstance = new Claim(ClaimName, "True");
    }
}