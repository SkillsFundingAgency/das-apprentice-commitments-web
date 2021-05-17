using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ConfirmApprenticeshipModel : PageModel
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; } = null!;

        public bool? EmployerConfirmation { get; set; } = null;
        public bool? TrainingProviderConfirmation { get; set; } = null;
        public bool? ApprenticeshipDetailsConfirmation { get; set; } = null;
        public bool? RolesAndResponsibilitiesConfirmation { get; set; } = null;
        public bool? HowApprenticeshipWillBeDeliveredConfirmation { get; set; } = null;

        public string Forwardlink => $"/apprenticeships/{ApprenticeshipId.Hashed}/transactioncomplete";

        public bool AllConfirmed
        {
            get
            {
                return EmployerConfirmation.Equals(true)
                    && TrainingProviderConfirmation.Equals(true)
                    && ApprenticeshipDetailsConfirmation.Equals(true)
                    && RolesAndResponsibilitiesConfirmation.Equals(true)
                    && HowApprenticeshipWillBeDeliveredConfirmation.Equals(true);
            }
        }

        public ConfirmApprenticeshipModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGetAsync()
        {
            _ = ApprenticeshipId ?? throw new PropertyNullException(nameof(ApprenticeshipId));

            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            EmployerConfirmation = apprenticeship.EmployerCorrect;
            TrainingProviderConfirmation = apprenticeship.TrainingProviderCorrect;
            ApprenticeshipDetailsConfirmation = apprenticeship.ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesConfirmation = apprenticeship.RolesAndResponsibilitiesCorrect;
            HowApprenticeshipWillBeDeliveredConfirmation = apprenticeship.HowApprenticeshipDeliveredCorrect;
        }

        public async Task<IActionResult> OnPostConfirm()
        {
            await _client.ConfirmApprenticeship(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id,
                new ApprenticeshipConfirmationRequest(true));

            return Redirect(Forwardlink);
        }
    }
}