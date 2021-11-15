using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ApprenticeshipIndexModel : PageModel
    {
        private readonly ApprenticeApi _client;
        private readonly IHashingService _hashing;

        public ApprenticeshipIndexModel(ApprenticeApi client, IHashingService hashing)
        {
            _client = client;
            _hashing = hashing;
        }

        public async Task<IActionResult> OnGet([FromServices] AuthenticatedUser user)
        {
            return await RedirectToLatestApprenticeship(user);
        }

        private async Task<IActionResult> RedirectToLatestApprenticeship(AuthenticatedUser user)
        {
            if (HttpContext.Request.Query.ContainsKey("RegistrationCode"))
                return RedirectToAction("Register", "Registration", new { RegistrationCode = HttpContext.Request.Query["RegistrationCode"] });

            var apprenticeship = await _client.TryGetApprenticeships(user.ApprenticeId);
            if (apprenticeship == null) return RedirectToPage("/Account");

            if (apprenticeship.Apprenticeships.Count == 0)
                return RedirectToPage("/CheckYourDetails");

            var firstApprenticeship = apprenticeship.Apprenticeships[0];
            var apprenticeshipId = _hashing.HashValue(firstApprenticeship.Id);
            return RedirectToPage("Confirm", new { apprenticeshipId });
        }
    }
}