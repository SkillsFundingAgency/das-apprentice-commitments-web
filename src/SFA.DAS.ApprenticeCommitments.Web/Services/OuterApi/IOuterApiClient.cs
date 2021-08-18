﻿using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient
    {
        [Get("/registrations/{id}")]
        Task<VerifyRegistrationResponse> GetRegistration([Path] Guid id);

        [AllowAnyStatusCode]
        [Post("/registrations/{registrationCode}/firstseen")]
        Task RegistrationFirstSeenOn([Path] string registrationCode, [Body] RegistrationFirstSeenOnRequest request);

        [Post("/registrations")]
        Task VerifyRegistration([Body] VerifyRegistrationRequest verification);

        [Get("/apprentices/{id}")]
        Task<Apprentice> GetApprentice([Path] Guid id);

        [Post("/apprentices")]
        Task CreateApprenticeAccount([Body] Apprentice apprentice);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprenticeship([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprenticeships")]
        Task ClaimApprenticeship([Body] ApprenticeshipAssociation association);

        [Get("/apprentices/{id}/apprenticeships")]
        Task<ApprenticeshipsWrapper> GetApprenticeships([Path] Guid id);

        [Get("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}")]
        Task<Apprenticeship> GetApprenticeship([Path] Guid apprenticeid, [Path] long apprenticeshipid);

        [Patch("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{revisionId}/confirmations")]
        Task ConfirmApprenticeship(
                 [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long revisionId,
                 [Body] ApprenticeshipConfirmationRequest confirmation);

        [Patch("/apprentices/{apprenticeId}/apprenticeships/{apprenticeshipId}")]
        Task UpdateApprenticeship(
            [Path] Guid apprenticeId, [Path] long apprenticeshipId,
            [Body] JsonPatchDocument<Apprenticeship> patch);
    }

    public static class OuterApiExtensions
    {
        public static async Task UpdateApprenticeshipLastViewed(this IOuterApiClient client, Guid apprenticeId, long apprenticeship, long commitmentStatementId)
        {
            var patch = new JsonPatchDocument<Apprenticeship>().Replace(x => x.LastViewed, DateTime.UtcNow);
            await client.UpdateApprenticeship(apprenticeId, apprenticeship, patch);
        }
    }
}