using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class ApprenticeApi
    {
        private readonly IOuterApiClient _client;

        public ApprenticeApi(IOuterApiClient client)
        {
            _client = client;
        }

        public async Task<Apprentice?> TryGetApprentice(Guid apprenticeId)
        {
            try
            {
                return await _client.GetApprentice(apprenticeId);
            }
            catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<ApprenticeshipsWrapper?> TryGetApprenticeships(Guid apprenticeId)
        {
            try
            {
                return await _client.GetApprenticeships(apprenticeId);
            }
            catch (ApiException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Apprentice> PutApprentice(string email, string govUkIdentifier)
        {
            return await _client.PutApprentice(new PutApprenticeAccount
            {
                Email = email,
                GovUkIdentifier = govUkIdentifier
            });
        }

        internal Task RegistrationSeen(string registrationCode, DateTime seenOn)
            => _client.RegistrationFirstSeenOn(registrationCode,
                   new RegistrationFirstSeenOnRequest { SeenOn = seenOn });

        internal async Task MatchApprenticeToApprenticeship(string registrationId, Guid apprenticeId)
        {
            await _client.ClaimApprenticeship(new ApprenticeshipAssociation
            {
                RegistrationId = registrationId,
                ApprenticeId = apprenticeId,
            });
        }
    }
}