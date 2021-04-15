using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [Authorize]
    public class ConfirmApprenticeshipModel : PageModel
    {
        private readonly AuthenticatedUserClient _client;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        public bool? EmployerConfirmation { get; set; } = null;
        public bool? TrainingProviderConfirmation { get; set; } = null;
        public bool? ApprenticeshipDetailsConfirmation { get; set; } = null;
        public bool? RolesAndResponsibilitiesConfirmation { get; set; } = null;
        public bool? HowApprenticeshipWillBeDeliveredConfirmation { get; set; } = null;

        public string Forwardlink => $"/apprenticeships/{ApprenticeshipId.Hashed}/transactioncomplete";

        public bool AllConfirmed
        {
            get {
                return EmployerConfirmation ==
                    TrainingProviderConfirmation ==
                    ApprenticeshipDetailsConfirmation ==
                    RolesAndResponsibilitiesConfirmation == true;
            }
        }

        public ConfirmApprenticeshipModel(AuthenticatedUserClient client)
        {
            _client = client;
        }

        public async Task OnGetAsync()
        {
            _ = ApprenticeshipId ?? throw new ArgumentNullException(nameof(ApprenticeshipId));

            var apprenticeship = await _client.GetApprenticeship(ApprenticeshipId.Id);
            
            EmployerConfirmation = apprenticeship.EmployerCorrect;
            TrainingProviderConfirmation = apprenticeship.TrainingProviderCorrect;
            ApprenticeshipDetailsConfirmation = apprenticeship.ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesConfirmation = apprenticeship.RolesAndResponsibilitiesCorrect;
            HowApprenticeshipWillBeDeliveredConfirmation = apprenticeship.HowApprenticeshipDeliveredCorrect;
        }
    }
}