using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    internal class MenuVisibility : IMenuVisibility
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;
        private Apprenticeship? _apprenticeship;

        public MenuVisibility(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task<bool> ShowConfirmMyApprenticeship() => true;

        public Task<bool> ShowApprenticeFeedback() => LatestApprenticeshipIsConfirmed();

        public async Task<ConfirmMyApprenticeshipTitleStatus> ConfirmMyApprenticeshipTitleStatus()
        {
            if (await LatestApprenticeshipIsConfirmed())
                return ApprenticePortal.SharedUi.Services.ConfirmMyApprenticeshipTitleStatus.ShowAsConfirmed;
            return ApprenticePortal.SharedUi.Services.ConfirmMyApprenticeshipTitleStatus.ShowAsRequiringConfirmation;
        } 

        private async Task<bool> LatestApprenticeshipIsConfirmed()
        {
            try
            {
                var latest = await GetLatestApprenticeship();

                var isConfirmed = latest?.ConfirmedOn.HasValue ?? false;

                return isConfirmed;
            }
            catch
            {
                return false;
            }
        }

        private async Task<Apprenticeship?> GetLatestApprenticeship()
        {
            if(_apprenticeship != null)
                return _apprenticeship;

            _apprenticeship = (await _client.GetApprenticeships(_authenticatedUser.ApprenticeId))?.Apprenticeships.FirstOrDefault();
            return _apprenticeship;
        }
    }
}