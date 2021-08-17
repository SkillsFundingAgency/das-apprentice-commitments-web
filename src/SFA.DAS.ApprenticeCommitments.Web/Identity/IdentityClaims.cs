using System.Linq;
using System.Security.Claims;

namespace SAF.DAS.ApprenticeCommitments.Web.Identity
{
    public static class IdentityClaims
    {
        public const string ApprenticeId = "apprentice_id";
        public const string LogonId = "sub";
        public const string Name = "name";
        public const string GivenName = "given_name";
        public const string FamilyName = "family_name";

        public static Claim? ApprenticeIdClaim(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(c => c.Type == ApprenticeId);

        public static Claim? EmailAddressClaim(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(x => IsName(x));

        public static string FullName(this ClaimsPrincipal user)
        {
            var first = user.Claims.FirstOrDefault(x => IsGivenName(x))?.Value ?? "";
            var last = user.Claims.FirstOrDefault(x => IsFamilyName(x))?.Value ?? "";
            return $"{first} {last}".Trim();
        }

        private static bool IsName(Claim x)
            => x.Type == Name || x.Type == ClaimTypes.Name;

        private static bool IsGivenName(Claim x)
            => x.Type == GivenName || x.Type == ClaimTypes.GivenName;

        private static bool IsFamilyName(Claim x)
            => x.Type == FamilyName || x.Type == ClaimTypes.Surname;

        public static ClaimsIdentity CreateApprenticeIdClaim(string id)
            => new ClaimsIdentity(new[] { new Claim(ApprenticeId, id) });
    }
}