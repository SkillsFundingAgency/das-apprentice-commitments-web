using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web
{
    public class RegistrationsService
    {
        private readonly ApiClient _client;

        public RegistrationsService(ApiClient client)
        {
            _client = client;
        }

        public Task<Registration> GetRegistration(Guid id)
        {
            return _client.GetRegistration(id);
        }
    }

    public interface ApiClient
    {
        [Get]
        Task<Registration> GetRegistration(Guid id);
    }

    public class Registration
    {
        public Guid Id { get; set; }
        public string RegistrationId = "";
        public string EmailAddress = "";
    }
}