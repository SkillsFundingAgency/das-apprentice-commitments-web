using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string TrainingProviderName { get; set; }

        [BindProperty]
        public bool? ConfirmedTrainingProvider { get; set; }

        public string BackLink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourTrainingModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser ?? throw new ArgumentNullException(nameof(authenticatedUser));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task OnGetAsync()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.RegistrationId, ApprenticeshipId.Id);
            TrainingProviderName = apprenticeship.TrainingProviderName;
            ConfirmedTrainingProvider = apprenticeship.TrainingProviderConfirmed;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedTrainingProvider == null)
            {
                ModelState.AddModelError(nameof(ConfirmedTrainingProvider), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmTrainingProvider(
                _authenticatedUser.RegistrationId, ApprenticeshipId.Id,
                new TrainingProviderConfirmationRequest(ConfirmedTrainingProvider.Value));

            var nextPage = ConfirmedTrainingProvider.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}