using Newtonsoft.Json;
using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public class RegistrationsService
    {
        private readonly IOuterApiClient _client;

        public RegistrationsService(IOuterApiClient client)
        {
            _client = client;
        }

        internal Task<VerifyRegistrationResponse> GetRegistration(AuthenticatedUser user) =>
            _client.GetRegistration(user.ApprenticeId);

        internal async Task VerifyRegistration(VerifyRegistrationRequest verification)
        {
            try
            {
                await _client.VerifyRegistration(verification);
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(ex.Content!);
                throw new DomainValidationException(errors);
            }
        }

        internal async Task FirstSeenOn(Guid apprenticeId, DateTime seenOn)
        {
            await _client.RegistrationFirstSeenOn(apprenticeId, new RegistrationFirstSeenOnRequest { SeenOn = seenOn });
        }

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