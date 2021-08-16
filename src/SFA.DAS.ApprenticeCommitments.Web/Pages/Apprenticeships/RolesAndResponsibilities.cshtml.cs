using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [HideNavigationBar]
    public class RolesAndResponsibilitiesModel : ApprenticeshipRevisionPageModel
    {
        private readonly AuthenticatedUserClient _client;

        [BindProperty]
        public bool? RolesAndResponsibilitiesConfirmed { get; set; } = null!;

        public RolesAndResponsibilitiesModel(AuthenticatedUserClient client)
        {
            _client = client;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client.GetApprenticeship(ApprenticeshipId.Id);

            RevisionId = apprenticeship.RevisionId;

            if (apprenticeship.RolesAndResponsibilitiesCorrect == true)
                RolesAndResponsibilitiesConfirmed = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (RolesAndResponsibilitiesConfirmed == null)
            {
                ModelState.AddModelError(nameof(RolesAndResponsibilitiesConfirmed), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmApprenticeship(ApprenticeshipId.Id, RevisionId,
                ApprenticeshipConfirmationRequest.ConfirmRolesAndResponsibilities(RolesAndResponsibilitiesConfirmed.Value));

            var nextPage = RolesAndResponsibilitiesConfirmed.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}