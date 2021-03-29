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
        public HashedId? ApprenticeshipId { get; set; }

        public bool? EmployerConfirmation { get; set; } = true;
        public bool? TrainingProviderConfirmation { get; set; } = false;
        public bool? ApprenticeshipConfirmation { get; set; } = null;

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
        }
    }
}