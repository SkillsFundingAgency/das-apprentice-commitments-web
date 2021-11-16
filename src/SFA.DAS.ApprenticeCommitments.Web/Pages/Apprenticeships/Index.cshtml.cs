using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.HashingService;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ApprenticeshipIndexModel : PageModel
    {
        private readonly ApprenticeApi _client;
        private readonly IHashingService _hashing;
        private readonly ILogger<ApprenticeshipIndexModel> _logger;

        public ApprenticeshipIndexModel(ApprenticeApi client, IHashingService hashing, ILogger<ApprenticeshipIndexModel> logger)
        {
            _client = client;
            _hashing = hashing;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet([FromServices] AuthenticatedUser user)
        {
            return await RedirectToLatestApprenticeship(user);
        }

        private async Task<IActionResult> RedirectToLatestApprenticeship(AuthenticatedUser user)
        {
            using (_logger.BeginPropertyScope(("ApprenticeId", user.ApprenticeId)))
            {
                if (Request.Cookies.TryGetValue("RegistrationCode", out var registrationCode))
                {
                    _logger.LogInformation("RedirectToLatestApprenticeship - Found RegistrationCode {RegistrationCode}", registrationCode);
                    return RedirectToAction("Register", "Registration", registrationCode);
                }

                var apprenticeship = await _client.TryGetApprenticeships(user.ApprenticeId);
                if (apprenticeship == null)
                {
                    _logger.LogInformation("RedirectToLatestApprenticeship - No account found");
                    return RedirectToPage("/Account");
                }

                if (apprenticeship.Apprenticeships.Count == 0)
                {
                    _logger.LogInformation("RedirectToLatestApprenticeship - No apprenticeships found");
                    return RedirectToPage("/CheckYourDetails");
                }

                var firstApprenticeship = apprenticeship.Apprenticeships[0];
                var apprenticeshipId = _hashing.HashValue(firstApprenticeship.Id);
                return RedirectToPage("Confirm", new { apprenticeshipId });
            }
        }
    }

    public static class ExEx
    {
        public static IDisposable BeginPropertyScope(
            this ILogger logger,
            params ValueTuple<string, object>[] properties)
        {
            var dictionary = properties.ToDictionary(p => p.Item1, p => p.Item2);
            return logger.BeginScope(dictionary);
        }

    }
}