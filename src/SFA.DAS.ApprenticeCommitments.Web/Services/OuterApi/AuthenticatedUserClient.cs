using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class AuthenticatedUserClient
    {
        private readonly IOuterApiClient client;
        private readonly AuthenticatedUser authenticatedUser;

        public AuthenticatedUserClient(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            this.authenticatedUser = authenticatedUser;
            this.client = client;
        }

        internal Task<Apprenticeship> GetApprenticeship(long id)
            => client.GetApprenticeship(authenticatedUser.ApprenticeId, id);

        internal Task<Apprenticeship> GetApprenticeshipRevision(long apprenticeshipId, long revisionId)
            => client.GetApprenticeshipRevision(authenticatedUser.ApprenticeId, apprenticeshipId, revisionId);

        internal Task ConfirmApprenticeship(long apprenticeshipId, long revisionId, ApprenticeshipConfirmationRequest confirmations)
            => client.ConfirmApprenticeship(authenticatedUser.ApprenticeId, apprenticeshipId, revisionId, confirmations);
    }
}