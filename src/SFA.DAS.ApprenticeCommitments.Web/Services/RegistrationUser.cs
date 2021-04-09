using SAF.DAS.ApprenticeCommitments.Web;
using System;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticatedUser
    {
        public AuthenticatedUser(ClaimsPrincipal user)
        {
            var claim = user.ApprenticeIdClaim()
                ?? throw new Exception("There is no `apprentice_id` claim.");

            if (!Guid.TryParse(claim.Value, out var apprenticeId))
                throw new Exception($"`{claim.Value}` in claim `apprentice_id` is not a valid identifier");

            ApprenticeId = apprenticeId;
        }

        public static AuthenticatedUser FakeUser => new AuthenticatedUser(FakeUserClaim);

        public static ClaimsPrincipal FakeUserClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new []
                {
                    new Claim("apprentice_id", Guid.NewGuid().ToString()),
                    new Claim("sub", Guid.NewGuid().ToString()),
                })
            });

        public Guid ApprenticeId { get; }
    }
}