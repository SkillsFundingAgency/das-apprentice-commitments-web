using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ConfirmYourTrainingModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string TrainingProviderName { get; set; } = null!;

        [BindProperty]
        public bool? ConfirmedTrainingProvider { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourTrainingModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser ?? throw new ArgumentNullException(nameof(authenticatedUser));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task OnGetAsync()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            TrainingProviderName = apprenticeship.TrainingProviderName;

            if (apprenticeship.TrainingProviderCorrect == true)
                ConfirmedTrainingProvider = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedTrainingProvider == null)
            {
                ModelState.AddModelError(nameof(ConfirmedTrainingProvider), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmTrainingProvider(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id,
                new TrainingProviderConfirmationRequest(ConfirmedTrainingProvider.Value));

            var nextPage = ConfirmedTrainingProvider.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}