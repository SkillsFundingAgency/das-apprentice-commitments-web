using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SAF.DAS.ApprenticeCommitments.Web;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticationEvents : OpenIdConnectEvents
    {
        private readonly VerifiedUserService _verifiedUserService;

        public AuthenticationEvents(VerifiedUserService verifiedUserService)
            => _verifiedUserService = verifiedUserService;

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await base.TokenValidated(context);
            await AddUserVerifiedClaim(context.Principal);
        }

        public async Task AddUserVerifiedClaim(ClaimsPrincipal p)
        {
            var claim = p.RegistationIdClaim();

            if (claim == null) return;
            if (!Guid.TryParse(claim.Value, out var registrationId)) return;
            if (!await _verifiedUserService.IsUserVerified(registrationId)) return;

            var identity = new ClaimsIdentity(new[] { new Claim("VerifiedUser", "True") });
            p.AddIdentity(identity);
        }
    }
}