using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class RegistrationsService
    {
        private readonly IOuterApiClient _client;

        public RegistrationsService(IOuterApiClient client)
        {
            _client = client;
        }

        public Task<VerifyRegistrationResponse> GetRegistration(Guid id) => _client.GetRegistration(id);

        internal Task<VerifyRegistrationResponse> GetRegistration(AuthenticatedUser user) =>
            GetRegistration(user.RegistrationId);

        internal async Task VerifyRegistration(VerifyRegistrationRequest verification)
        {
            try
            {
                await _client.VerifyRegistration(verification);
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(ex.Content);
                throw new DomainValidationException(errors);
            }
        }
    }

    internal class DomainValidationException : Exception
    {
        public List<ErrorItem> Errors { get; }

        public DomainValidationException(List<ErrorItem> errors) : base("DomainValidation Exception")
        {
            Errors = errors;
        }

        public DomainValidationException(string message) : this(new List<ErrorItem>{new ErrorItem { ErrorMessage = message}})
        {
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class ErrorItem
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}