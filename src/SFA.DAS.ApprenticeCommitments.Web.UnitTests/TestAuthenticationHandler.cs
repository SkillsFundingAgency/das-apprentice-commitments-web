using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TestAuthenticationHandler : SignInAuthenticationHandler<AuthenticationSchemeOptions>
    {
        public enum AccountStatus
        {
            Initial,
            AccountCreated,
            TermsAccepted,
        }

        private static readonly ConcurrentDictionary<Guid, AccountStatus> _users
            = new ConcurrentDictionary<Guid, AccountStatus>();

        public static List<ClaimsPrincipal> Authentications { get; } = new List<ClaimsPrincipal>();

        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        public static void AddUserWithFullAccount(Guid apprenticeId)
        {
            Console.WriteLine($"Adding logged in user {apprenticeId}");
            _users.TryAdd(apprenticeId, AccountStatus.TermsAccepted);
        }

        public static void AddUserWithoutTerms(Guid apprenticeId)
        {
            Console.WriteLine($"Adding logged in user {apprenticeId} who hasn't accepts ToC");
            _users.TryAdd(apprenticeId, AccountStatus.AccountCreated);
        }

        internal static void AddUserWithoutAccount(Guid apprenticeId)
        {
            Console.WriteLine($"Adding unverified logged in user {apprenticeId}");
            _users.TryAdd(apprenticeId, AccountStatus.Initial);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(HandleAuthenticate());
        }

        protected AuthenticateResult HandleAuthenticate()
        {
            var guid = FindUserFromHeader();
            if (guid == null) return AuthenticateResult.Fail("No user header found");

            var exists = _users.TryGetValue(guid.Value, out var status);
            if (!exists) return AuthenticateResult.Fail($"User `{guid}` is not logged in");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Testuser@example.com"),
                new Claim("apprentice_id", guid.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "Test1");
            var principal = new ClaimsPrincipal(identity);

            if (status >= AccountStatus.AccountCreated)
                principal.AddAccountCreatedClaim();

            if (status >= AccountStatus.TermsAccepted)
                principal.AddTermsOfUseAcceptedClaim();

            var ticket = new AuthenticationTicket(principal, "Test2");

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleSignInAsync(ClaimsPrincipal principal, AuthenticationProperties properties)
        {
            var guid = principal.Claims.First(x => x.Type == "apprentice_id").Value;
            var userId = Guid.Parse(guid);

            Authentications.Add(principal);

            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties) => Task.CompletedTask;

        private Guid? FindUserFromHeader()
        {
            if (Request.Headers.TryGetValue("Authorization", out var value) && Guid.TryParse(value, out var guid))
                return guid;
            return default;
        }
    }
}