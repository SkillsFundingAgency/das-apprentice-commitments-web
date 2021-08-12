using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public abstract class ApprenticeshipConfirmationPageModel : ApprenticeshipRevisionPageModel
    {
        private readonly AuthenticatedUserClient _client;
        private readonly string _entityName;

        protected ApprenticeshipConfirmationPageModel(string entityName, AuthenticatedUserClient client)
        {
            _client = client;
            _entityName = entityName;
        }

        [BindProperty]
        public bool? Confirmed { get; set; }

        public bool ChangingAnswer { get; protected set; }

        public bool CanChangeAnswer { get; protected set; }

        public bool ShowForm => Confirmed != true || ChangingAnswer;

        public async Task OnGetAsync()
        {
            var apprenticeship = await _client.GetApprenticeship(ApprenticeshipId.Id);
            RevisionId = apprenticeship.CommitmentStatementId;
            LoadApprenticeship(apprenticeship);
            CanChangeAnswer = Confirmed == true && !apprenticeship.IsCompleted();
        }

        public async Task OnGetChangeAnswer()
        {
            await OnGetAsync();
            if (CanChangeAnswer)
            {
                ChangingAnswer = true;
                CanChangeAnswer = false;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (Confirmed == null)
            {
                ModelState.AddModelError(nameof(Confirmed), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmApprenticeship(
                ApprenticeshipId.Id, RevisionId,
                CreateUpdate(Confirmed.Value));

            var nextPage = Confirmed.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, null, new { ApprenticeshipId, Entity = _entityName });
        }

        public abstract void LoadApprenticeship(Apprenticeship apprenticeship);

        public abstract ApprenticeshipConfirmationRequest CreateUpdate(bool confirmed);
    }
}