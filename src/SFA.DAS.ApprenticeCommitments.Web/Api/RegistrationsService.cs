using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public class RegistrationsService
    {
        private readonly IApiClient _client;

        public RegistrationsService(IApiClient client) => _client = client;

        public Task<Registration> GetRegistration(Guid id) => _client.GetRegistration(id);

        internal Task<Registration> GetRegistration(RegistrationUser user) =>
            GetRegistration(user.RegistrationId);

        internal Task Validate(VerifyRegistrationCommand verification) =>
            _client.Validate(verification.RegistrationId, verification);
    }
}