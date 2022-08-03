using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [HideNavigationBar]
    public class HowYourApprenticeshipWillBeDeliveredModel : ApprenticeshipRevisionPageModel
    {
        private readonly AuthenticatedUserClient _client;

        [BindProperty]
        public bool? ConfirmedHowApprenticeshipDelivered { get; set; } = null!;

        public HowYourApprenticeshipWillBeDeliveredModel(AuthenticatedUserClient client)
        {
            _client = client;
        }

        public async Task OnGet(long? revisionId = null)
        {
            var apprenticeship = await OnGetAsync(_client, revisionId);

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

            await _client.ConfirmApprenticeship(ApprenticeshipId.Id, RevisionId,
                ApprenticeshipConfirmationRequest.ConfirmDelivery(ConfirmedHowApprenticeshipDelivered.Value));

            var nextPage = ConfirmedHowApprenticeshipDelivered.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}