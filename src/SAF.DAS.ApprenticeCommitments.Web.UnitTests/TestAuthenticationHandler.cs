﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TestAuthenticationHandler : SignInAuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static readonly ConcurrentDictionary<Guid, bool> _users = new ConcurrentDictionary<Guid, bool>();

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        public static void AddUser(Guid registrationId)
        {
            Console.WriteLine($"Adding logged in user {registrationId}");
            _users.TryAdd(registrationId, true);
        }

        internal static void AddUnverifiedUser(Guid apprenticeId)
        {
            Console.WriteLine($"Adding unverified logged in user {apprenticeId}");
            _users.TryAdd(apprenticeId, false);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(HandleAuthenticate());
        }

        protected AuthenticateResult HandleAuthenticate()
        {
            var guid = FindUserFromHeader();
            if (guid == null) return AuthenticateResult.Fail("No user header found");

            var exists = _users.TryGetValue(guid.Value, out var isVerified);
            if (!exists) return AuthenticateResult.Fail($"User `{guid}` is not logged in");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Testuser@example.com"),
                new Claim("apprentice_id", guid.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "Test1");
            if (isVerified) identity.AddAccountCreatedClaim();
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test2");

            return AuthenticateResult.Success(ticket);
        }

        private Guid? FindUserFromHeader()
        {
            if (Request.Headers.TryGetValue("Authorization", out var value) && Guid.TryParse(value, out var guid))
                return guid;
            return default;
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties) => Task.CompletedTask;

        protected override Task HandleSignOutAsync(AuthenticationProperties properties) => Task.CompletedTask;
    }
}