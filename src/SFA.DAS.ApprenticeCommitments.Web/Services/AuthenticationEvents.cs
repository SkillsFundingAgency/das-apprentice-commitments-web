using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SAF.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticationEvents : OpenIdConnectEvents
    {
        private readonly ApprenticeApi _client;

        public AuthenticationEvents(ApprenticeApi client) => _client = client;

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await base.TokenValidated(context);
            ConvertRegistrationIdToApprenticeId(context.Principal);
            await AddClaims(context.Principal);
        }

        public void ConvertRegistrationIdToApprenticeId(ClaimsPrincipal principal)
        {
            var registrationClaim = principal.Claims.FirstOrDefault(c => c.Type == "registration_id");
            var apprenticeClaim = principal.ApprenticeIdClaim();

            if (registrationClaim == null) return;
            if (apprenticeClaim != null) return;

            principal.AddIdentity(IdentityClaims.CreateApprenticeIdClaim(registrationClaim.Value));
        }

        public async Task AddClaims(ClaimsPrincipal principal)
        {
            var apprentice = await GetApprentice(principal);
            if (apprentice == null) return;

            AddAccountCreatedClaim(principal);
            AddApprenticeNameClaims(apprentice, principal);
        }

        private async Task<Apprentice?> GetApprentice(ClaimsPrincipal principal)
        {
            var claim = principal.ApprenticeIdClaim();

            if (claim == null) return null;
            if (!Guid.TryParse(claim.Value, out var apprenticeId)) return null;

            return await _client.TryGetApprentice(apprenticeId);
        }

        private void AddAccountCreatedClaim(ClaimsPrincipal principal)
            => principal.AddIdentity(UserAccountCreatedClaim.CreateAccountCreatedClaim());

        private void AddApprenticeNameClaims(Apprentice apprentice, ClaimsPrincipal principal)
            => principal.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(IdentityClaims.GivenName, apprentice.FirstName),
                new Claim(IdentityClaims.FamilyName, apprentice.LastName),
            }));
    }
}