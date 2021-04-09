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
            var claim = user.RegistationIdClaim()
                ?? throw new Exception("There is no `registration_id` claim.");

            if (!Guid.TryParse(claim.Value, out var registrationId))
                throw new Exception($"`{claim.Value}` in claim `registration_id` is not a valid identifier");

            ApprenticeId = registrationId;
        }

        public static AuthenticatedUser FakeUser => new AuthenticatedUser(FakeUserClaim);

        public static ClaimsPrincipal FakeUserClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new []
                {
                    new Claim("registration_id", Guid.NewGuid().ToString()),
                    new Claim("sub", Guid.NewGuid().ToString()),
                })
            });

        public Guid ApprenticeId { get; }
    }
}