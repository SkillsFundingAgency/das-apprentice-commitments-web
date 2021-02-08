using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public class RegistrationsService
    {
        private readonly IApiClient _client;

        public RegistrationsService(IApiClient client) => _client = client;

        public Task<Registration> GetRegistration(Guid id) => _client.GetRegistration(id);
    }
}