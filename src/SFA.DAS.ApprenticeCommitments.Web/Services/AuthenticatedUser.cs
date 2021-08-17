using SAF.DAS.ApprenticeCommitments.Web.Identity;
using System;
using System.Net.Mail;
using System.Security.Claims;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticatedUser
    {
        public AuthenticatedUser(ClaimsPrincipal user)
        {
            var claim = user.ApprenticeIdClaim()
                ?? throw new InvalidOperationException($"There is no `{IdentityClaims.ApprenticeId}` claim.");

            if (!Guid.TryParse(claim.Value, out var apprenticeId))
                throw new InvalidOperationException($"`{claim.Value}` in claim `{IdentityClaims.ApprenticeId}` is not a valid identifier");

            ApprenticeId = apprenticeId;

            var emailClaim = user.EmailAddressClaim()
                ?? throw new InvalidOperationException($"There is no `{IdentityClaims.Name}` claim for the email.");

            Email = new MailAddress(emailClaim.Value ?? "");
        }

        public static AuthenticatedUser FakeUser => new AuthenticatedUser(FakeUserClaim);

        public static ClaimsPrincipal FakeUserClaim =>
            new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new []
                {
                    new Claim(IdentityClaims.ApprenticeId, Guid.NewGuid().ToString()),
                    new Claim(IdentityClaims.Name, "bob@example.com"),
                    new Claim(IdentityClaims.LogonId, Guid.NewGuid().ToString()),
                    new Claim(UserAccountCreatedClaim.ClaimName, "True"),
                })
            });

        public Guid ApprenticeId { get; }

        public MailAddress Email { get; }
    }
}