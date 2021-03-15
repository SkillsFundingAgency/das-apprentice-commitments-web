using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingModel : PageModel
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        public string TrainingProviderName { get; private set; }
        
        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourTrainingModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
            _client = client;
        }

        public async Task OnGetAsync()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.RegistrationId, ApprenticeshipId.Id);
            TrainingProviderName = apprenticeship.TrainingProviderName;
        }
    }
}