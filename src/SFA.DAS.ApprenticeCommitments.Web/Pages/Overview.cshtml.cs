using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [Authorize]
    public class OverviewModel : PageModel
    {
        private readonly IOuterApiClient _client;

        public OverviewModel(IOuterApiClient client) => _client = client;

        [BindProperty(SupportsGet = true)]
        public long? ApprenticeshipId { get; set; }

        public async Task<IActionResult> OnGet([FromServices] AuthenticatedUser user)
        {
            return ApprenticeshipId == null
                ? await RedirectToLatestApprenticeship(user)
                : Page();
        }

        private async Task<IActionResult> RedirectToLatestApprenticeship(AuthenticatedUser user)
        {
            var apprenticeship = await _client.GetCurrentApprenticeship(user.RegistrationId);
            return Redirect($"/Overview/{apprenticeship.Id}");
        }
    }
}