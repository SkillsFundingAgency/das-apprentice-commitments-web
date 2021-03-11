using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.HashingService;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourEmployerModel : PageModel
    {
        private readonly IHashingService _hashingService;
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public string? ApprenticeshipId { get; set; }
        public string EmployerName { get; set; }
        public string Backlink { get; set; }

        public ConfirmYourEmployerModel(IHashingService hashingService, IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _hashingService = hashingService;
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            Backlink = $"/apprenticeships/{ApprenticeshipId}/confirm";
            var apprenticeshipId = _hashingService.DecodeValue(ApprenticeshipId);
            var apprenticeship = await _client.GetApprenticeship(_authenticatedUser.RegistrationId, apprenticeshipId);
            EmployerName = apprenticeship.EmployerName;
        }
    }
}
