﻿using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SAF.DAS.ApprenticeCommitments.Web;
using System;
using System.Linq;
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
            ConvertRegistrationIdToApprenticeId(context.Principal);
            await AddUserVerifiedClaim(context.Principal);
        }

        public void ConvertRegistrationIdToApprenticeId(ClaimsPrincipal principal)
        {
            var registrationClaim = principal.Claims.FirstOrDefault(c => c.Type == "registration_id");
            var apprenticeClaim = principal.ApprenticeIdClaim();

            if (registrationClaim == null) return;
            if (apprenticeClaim != null) return;

            var apprenticeId = new ClaimsIdentity(new[] { new Claim("apprentice_id", registrationClaim.Value) });
            principal.AddIdentity(apprenticeId);
        }

        public async Task AddUserVerifiedClaim(ClaimsPrincipal principal)
        {
            var claim = principal.ApprenticeIdClaim();

            if (claim == null) return;
            if (!Guid.TryParse(claim.Value, out var registrationId)) return;
            if (!await _verifiedUserService.IsUserVerified(registrationId)) return;

            var verifiedUser = new ClaimsIdentity(new[] { new Claim("VerifiedUser", "True") });
            principal.AddIdentity(verifiedUser);
        }
    }
}