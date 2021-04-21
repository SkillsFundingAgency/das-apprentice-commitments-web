using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [Authorize]
    public class ApprenticeshipIndexModel : PageModel
    {
        private readonly IOuterApiClient _client;
        private readonly IHashingService _hashing;

        public ApprenticeshipIndexModel(IOuterApiClient client, IHashingService hashing)
        {
            _client = client;
            _hashing = hashing;
        }

        public async Task<IActionResult> OnGet([FromServices] AuthenticatedUser user)
        {
            try
            {
                return await RedirectToLatestApprenticeship(user);
            }
            catch (RestEase.ApiException e) when(e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return RedirectToPage("/ConfirmYourIdentity");
            }
        }

        private async Task<IActionResult> RedirectToLatestApprenticeship(AuthenticatedUser user)
        {
            var apprenticeship = await _client.GetApprenticeships(user.ApprenticeId);
            var firstApprenticeship = apprenticeship[0];
            var hashedId = _hashing.HashValue(firstApprenticeship.Id);
            return RedirectToPage("Confirm", new { apprenticeshipId = hashedId });
        }
    }
}