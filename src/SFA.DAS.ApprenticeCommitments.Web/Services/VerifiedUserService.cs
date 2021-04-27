using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class VerifiedUserService
    {
        private readonly IOuterApiClient _client;

        public VerifiedUserService(IOuterApiClient client) => _client = client;

        public async Task<bool> IsUserVerified(Guid guid)
        {
            try
            {
                var registration = await _client.GetRegistration(guid);
                return registration.HasCompletedVerification;
            }
            catch
            {
                return false;
            }
        }
    }
}