using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
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

            AddClaims(principal, apprentice);
        }
        private async Task<Apprentice?> GetApprentice(ClaimsPrincipal principal)
        {
            var claim = principal.ApprenticeIdClaim();

            if (Guid.TryParse(claim?.Value, out var apprenticeId))
                return await _client.TryGetApprentice(apprenticeId);
            else
                return null;
        }

        private static void AddClaims(ClaimsPrincipal principal, Apprentice apprentice)
        {
            AddAccountCreatedClaim(principal);
            AddApprenticeNameClaims(apprentice, principal);
        }

        private static void AddAccountCreatedClaim(ClaimsPrincipal principal)
            => principal.AddIdentity(UserAccountCreatedClaim.CreateAccountCreatedClaim());

        private static void AddApprenticeNameClaims(Apprentice apprentice, ClaimsPrincipal principal)
            => principal.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(IdentityClaims.GivenName, apprentice.FirstName),
                new Claim(IdentityClaims.FamilyName, apprentice.LastName),
            }));

        public static async Task UserAccountCreated(HttpContext context, Apprentice apprentice)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                AddClaims(authenticated.Principal, apprentice);
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }

        internal static async Task TermsOfUseAccepted(HttpContext context)
        {
            var authenticated = await context.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                authenticated.Principal.AddIdentity(TermsOfUseAcceptedClaim.CreateTermsOfUseAcceptedClaim());
                await context.SignInAsync(authenticated.Principal, authenticated.Properties);
            }
        }
    }
}