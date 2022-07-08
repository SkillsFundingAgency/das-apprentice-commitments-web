using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Filters;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ViewMyApprenticeshipModel : PageModel
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;
        private readonly ILogger<ViewMyApprenticeshipModel> _logger;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }
        public Apprenticeship LatestConfirmedApprenticeship { get; set; } = null!;

        public ViewMyApprenticeshipModel(IOuterApiClient client, ITimeProvider time, AuthenticatedUser authenticatedUser, ILogger<ViewMyApprenticeshipModel> logger)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (ApprenticeshipId == default)
                throw new PropertyNullException(nameof(ApprenticeshipId));

            _logger.LogInformation("Getting confirmed apprenticeship {apprenticeshipId}", ApprenticeshipId.Id);
            try
            {
                var apprenticeship =
                    await _client.GetMyApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);
                LatestConfirmedApprenticeship = apprenticeship;
            }
            catch (RestEase.ApiException e)
            {
                _logger.LogError("RestEase API exception", e);
                _logger.LogError("No confirmed apprenticeship found for {id}", ApprenticeshipId.Id);
                throw;
            }
        }
    }
}