using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    internal class MenuVisibility : IMenuVisibility
    {
        private readonly IOuterApiClient client;
        private readonly AuthenticatedUser authenticatedUser;

        public MenuVisibility(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            this.client = client;
            this.authenticatedUser = authenticatedUser;
        }

        public async Task<bool> ShowConfirmMyApprenticeship() => true;

        public async Task<bool> ShowApprenticeFeedback()
        {
            try
            {
                var apprentice = await client.GetApprentice(authenticatedUser.ApprenticeId);
                var apprenticeship = (await client.GetApprenticeships(authenticatedUser.ApprenticeId))?.Apprenticeships.FirstOrDefault();

                var isConfirmed = apprenticeship?.ConfirmedOn.HasValue ?? false;
                
                return isConfirmed;
            }
            catch
            {
                return false;
            }
        }
    }
}