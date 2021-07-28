using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    [HideNavigationBar]
    public class ConfirmYourEmployerModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string EmployerName { get; set; } = null!;

        [BindProperty]
        public bool? ConfirmedEmployer { get; set; }

        public bool ShowForm => ConfirmedEmployer != true || ChangingAnswer;

        public bool ChangingAnswer { get; private set; }

        public bool CanChangeAnswer { get; private set; }

        [BindProperty]
        public long CommitmentStatementId { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourEmployerModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            CommitmentStatementId = apprenticeship.CommitmentStatementId;
            EmployerName = apprenticeship.EmployerName;
            ConfirmedEmployer = apprenticeship.EmployerCorrect;
            CanChangeAnswer = ConfirmedEmployer == true && !apprenticeship.IsCompleted();
        }

        public async Task OnGetChangeAnswer()
        {
            await OnGet();
            if (CanChangeAnswer)
            {
                ChangingAnswer = true;
                CanChangeAnswer = false;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedEmployer == null)
            {
                ModelState.AddModelError(nameof(ConfirmedEmployer), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmEmployer(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id, CommitmentStatementId,
                new EmployerConfirmationRequest(ConfirmedEmployer.Value));

            var nextPage = ConfirmedEmployer.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, null, new { ApprenticeshipId = ApprenticeshipId, Entity = "Employer" });
        }
    }
}