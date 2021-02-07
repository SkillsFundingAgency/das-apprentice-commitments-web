using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public class RegistrationsService
    {
        private readonly ApiClient _client;

        public RegistrationsService(ApiClient client) => _client = client;

        public Task<Registration> GetRegistration(Guid id) => _client.GetRegistration(id);
    }
}