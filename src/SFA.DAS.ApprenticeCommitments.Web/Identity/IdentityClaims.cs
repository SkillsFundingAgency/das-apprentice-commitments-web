using System.Linq;
using System.Security.Claims;

namespace SAF.DAS.ApprenticeCommitments.Web.Identity
{
    public static class IdentityClaims
    {
        public const string ApprenticeId = "apprentice_id";
        public const string VerifiedUser = "VerifiedUser";
        public const string LogonId = "sub";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";

        public static Claim? ApprenticeIdClaim(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(c => c.Type == ApprenticeId);

        public static string FullName(this ClaimsPrincipal user)
        {
            var first = user.Claims.FirstOrDefault(x => IsGivenName(x))?.Value ?? "";
            var last = user.Claims.FirstOrDefault(x => IsFamilyName(x))?.Value ?? "";
            return $"{first} {last}".Trim();
        }

        private static bool IsGivenName(Claim x)
            => x.Type == GivenName || x.Type == ClaimTypes.GivenName;

        private static bool IsFamilyName(Claim x)
            => x.Type == FamilyName || x.Type == ClaimTypes.Surname;

        public static ClaimsIdentity CreateApprenticeIdClaim(string id)
            => new ClaimsIdentity(new[] { new Claim(ApprenticeId, id) });

        internal static ClaimsIdentity CreateVerifiedUserClaim(bool verified)
            => new ClaimsIdentity(new[] { new Claim(VerifiedUser, verified.ToString()) });
    }
}