using System.Linq;
using System.Security.Claims;

#nullable enable

namespace SAF.DAS.ApprenticeCommitments.Web
{
    public static class UserNameExtension
    {
        public static string FullName(this ClaimsPrincipal user)
        {
            var first = user.Claims.FirstOrDefault(x => IsGivenName(x))?.Value ?? "";
            var last = user.Claims.FirstOrDefault(x => IsFamilyName(x))?.Value ?? "";
            return $"{first} {last}".Trim();
        }

        private static bool IsGivenName(Claim x)
            => x.Type == "given_name" || x.Type == ClaimTypes.GivenName;

        private static bool IsFamilyName(Claim x)
            => x.Type == "family_name" || x.Type == ClaimTypes.Surname;
    }
}