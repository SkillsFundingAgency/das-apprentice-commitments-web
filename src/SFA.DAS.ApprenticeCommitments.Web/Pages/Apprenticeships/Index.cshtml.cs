using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ApprenticeshipIndexModel : PageModel
    {
        private readonly ApprenticeApi _client;
        private readonly IHashingService _hashing;
        private readonly NavigationUrlHelper _urlHelper;

        public ApprenticeshipIndexModel(ApprenticeApi client, IHashingService hashing, NavigationUrlHelper urlHelper)
        {
            _client = client;
            _hashing = hashing;
            _urlHelper = urlHelper;
        }

        public async Task<IActionResult> OnGet([FromServices] AuthenticatedUser user)
        {
            return await RedirectToLatestApprenticeship(user);
        }

        private async Task<IActionResult> RedirectToLatestApprenticeship(AuthenticatedUser user)
        {
            var apprenticeship = await _client.TryGetApprenticeships(user.ApprenticeId);
            if (apprenticeship == null) return RedirectToPage("/Account");

            if (apprenticeship.Apprenticeships.Count == 0)
                return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home?notification=ApprenticeshipDidNotMatch"));

            var firstApprenticeship = apprenticeship.Apprenticeships[0];
            var apprenticeshipId = _hashing.HashValue(firstApprenticeship.Id);
            return RedirectToPage("Confirm", new { apprenticeshipId });
        }
    }
}