using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.HashingService;
using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ApprenticeshipIndexModel : PageModel
    {
        private readonly ApprenticeApi _client;
        private readonly IHashingService _hashing;
        private readonly ILogger<ApprenticeshipIndexModel> _logger;
        private readonly NavigationUrlHelper _urlHelper;

        public ApprenticeshipIndexModel(ApprenticeApi client, IHashingService hashing, ILogger<ApprenticeshipIndexModel> logger, NavigationUrlHelper urlHelper)
        {
            _client = client;
            _hashing = hashing;
            _logger = logger;
            _urlHelper = urlHelper;
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
                if (apprenticeship == null) return Redirect(_urlHelper.Generate(NavigationSection.PersonalDetails));

                if (apprenticeship.Apprenticeships.Count == 0)
                    return RedirectToPage("/CheckYourDetails");

                var firstApprenticeship = apprenticeship.Apprenticeships[0];
                var apprenticeshipId = _hashing.HashValue(firstApprenticeship.Id);
                return RedirectToPage("Confirm", new { apprenticeshipId });
            }
        }
    }
}