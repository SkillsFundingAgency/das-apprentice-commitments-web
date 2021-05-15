using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class HowYourApprenticeshipWillBeDeliveredModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; } = null!;

        [BindProperty]
        public bool? ConfirmedHowApprenticeshipDelivered { get; set; } = null!;

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public HowYourApprenticeshipWillBeDeliveredModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            if (apprenticeship.HowApprenticeshipDeliveredCorrect == true)
                ConfirmedHowApprenticeshipDelivered = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedHowApprenticeshipDelivered == null)
            {
                ModelState.AddModelError(nameof(ConfirmedHowApprenticeshipDelivered), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmHowApprenticeshipDelivered(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id,
                new HowApprenticeshipDeliveredConfirmationRequest(ConfirmedHowApprenticeshipDelivered.Value));

            var nextPage = ConfirmedHowApprenticeshipDelivered.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}