using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticePortal.Authentication;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class ApprenticeAccountProvider : IApprenticeAccountProvider
    {
        private readonly ApprenticeApi _client;

        public ApprenticeAccountProvider(ApprenticeApi client)
        {
            _client = client;
        }

        public async Task<IApprenticeAccount?> GetApprenticeAccount(Guid id)
        {
            return await _client.TryGetApprentice(id);
        }

        public async Task<IApprenticeAccount?> PutApprenticeAccount(string email, string govUkIdentifier)
        {
            return await _client.PutApprentice(email, govUkIdentifier);
        }
    }
}