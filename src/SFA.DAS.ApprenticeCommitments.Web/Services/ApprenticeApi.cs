﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
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

        internal async Task UpdateApprentice(Guid apprenticeId, string firstName, string lastName, DateTime? dateOfBirth = null)
        {
            await TryApi(async () =>
            {
                var patch = new JsonPatchDocument<Apprentice>()
                    .Replace(x => x.FirstName, firstName)
                    .Replace(x => x.LastName, lastName);

                if (dateOfBirth != null)
                    patch = patch.Replace(x => x.DateOfBirth, dateOfBirth);

                await _client.UpdateApprentice(apprenticeId, patch);
            });
        }

        internal async Task AcceptTermsOfUse(Guid apprenticeId)
        {
            await TryApi(async () =>
            {
                await _client.UpdateApprentice(apprenticeId,
                    new JsonPatchDocument<Apprentice>().Replace(x => x.TermsOfUseAccepted, true));
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
                var value = ex.Content!;
                var errors = JsonConvert.DeserializeObject<ValidationProblemDetails>(value);
                throw new DomainValidationException(errors);
            }
        }
    }
}