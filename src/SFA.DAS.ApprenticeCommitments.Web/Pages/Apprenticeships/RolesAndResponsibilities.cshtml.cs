using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [HideNavigationBar]
    public class RolesAndResponsibilitiesModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public bool? RolesAndResponsibilitiesConfirmed { get; set; }

        [BindProperty]
        public long CommitmentStatementId { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public RolesAndResponsibilitiesModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            CommitmentStatementId = apprenticeship.CommitmentStatementId;

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

            await _client.ConfirmRolesAndResponsibilities(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id, CommitmentStatementId,
                new RolesAndResponsibilitiesConfirmationRequest(RolesAndResponsibilitiesConfirmed.Value));

            var nextPage = RolesAndResponsibilitiesConfirmed.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}