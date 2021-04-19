using SAF.DAS.ApprenticeCommitments.Web.Identity;
using System;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticatedUser
    {
        public AuthenticatedUser(ClaimsPrincipal user)
        {
            var claim = user.ApprenticeIdClaim()
                ?? throw new Exception($"There is no `{IdentityClaims.ApprenticeId}` claim.");

            if (!Guid.TryParse(claim.Value, out var apprenticeId))
                throw new Exception($"`{claim.Value}` in claim `{IdentityClaims.ApprenticeId}` is not a valid identifier");

            ApprenticeId = apprenticeId;
        }

        public static AuthenticatedUser FakeUser => new AuthenticatedUser(FakeUserClaim);

        public static ClaimsPrincipal FakeUserClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new []
                {
                    new Claim(IdentityClaims.ApprenticeId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.LogonId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.VerifiedUser, "True"),
                })
            });

        public Guid ApprenticeId { get; }
    }
}