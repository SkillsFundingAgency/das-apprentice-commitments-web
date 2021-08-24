using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Collections.Generic;
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

        internal Task RegistrationSeen(string registrationCode, DateTime seenOn)
            => _client.RegistrationFirstSeenOn(registrationCode,
                   new RegistrationFirstSeenOnRequest { SeenOn = seenOn });

        internal async Task CreateApprentice(Apprentice apprentice)
        {
            await TryApi(() => _client.CreateApprenticeAccount(apprentice));
        }

        internal async Task UpdateApprentice(Guid apprenticeId, string firstName, string lastName, DateTime dateOfBirth)
        {
            await TryApi(async () =>
            {
                var patch = new JsonPatchDocument<Apprentice>()
                    .Replace(x => x.FirstName, firstName)
                    .Replace(x => x.LastName, lastName)
                    .Replace(x => x.DateOfBirth, dateOfBirth);

                await _client.UpdateApprentice(apprenticeId, patch);
            });
        }

        internal async Task MatchApprenticeToApprenticeship(string registrationId, Guid apprenticeId)
        {
            await _client.ClaimApprenticeship(new ApprenticeshipAssociation
            {
                RegistrationId = registrationId,
                ApprenticeId = apprenticeId,
            });
        }

        private async Task TryApi(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errors = JsonConvert.DeserializeObject<List<ErrorItem>>(ex.Content!);
                throw new DomainValidationException(errors);
            }
        }
    }
}