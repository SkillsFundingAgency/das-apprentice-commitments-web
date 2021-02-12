using Newtonsoft.Json;
using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public class RegistrationsService
    {
        private readonly IApiClient _client;

        public RegistrationsService(IApiClient client) => _client = client;

        public Task<Registration> GetRegistration(Guid id) => _client.GetRegistration(id);

        internal Task<Registration> GetRegistration(AuthenticatedUser user) =>
            GetRegistration(user.RegistrationId);

        internal async Task VerifyRegistration(VerifyRegistrationCommand verification)
        {
            try
            {
                await _client.VerifyRegistration(verification);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(ex.Content);

                    throw new DomainValidationException(errors);
                }

                throw;
            }
            catch
            {
                throw;
            }
        }
    }

    [Serializable]
    internal class DomainValidationException : Exception
    {
        public List<ErrorItem> Errors { get; }

        public DomainValidationException()
        {
        }

        public DomainValidationException(List<ErrorItem> errors) => Errors = errors;

        public DomainValidationException(string message) : base(message)
        {
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DomainValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class ErrorItem
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}