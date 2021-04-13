using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

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
    }
}