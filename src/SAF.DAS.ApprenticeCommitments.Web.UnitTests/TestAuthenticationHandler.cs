using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static ConcurrentBag<Guid> _users = new ConcurrentBag<Guid>();

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
            _users.Add(registrationId);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(HandleAuthenticate());
        }

        protected AuthenticateResult HandleAuthenticate()
        {
            var guid = FindUserFromHeader();
            if(guid == null) return AuthenticateResult.Fail("No user header found");

            var exists = _users.TryPeek(out var _);
            if (!exists) return AuthenticateResult.Fail($"User `{guid}` is not logged in");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim("registration_id", guid.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "Test1");
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
    }
}