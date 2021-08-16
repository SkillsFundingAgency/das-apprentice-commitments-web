using Microsoft.AspNetCore.JsonPatch;
using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient : IObsoleteApi
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

        [Get("/apprentices/{id}")]
        Task<Response<Apprentice>> TryGetApprenticeAccount([Path] Guid id);

        [Post("/apprentices")]
        Task CreateApprenticeAccount([Body] Apprentice apprentice);

        [Patch("/apprentices/{apprenticeId}")]
        Task UpdateApprenticeship([Path] Guid apprenticeId, [Body] JsonPatchDocument<Apprentice> patch);

        [Post("/apprenticeships")]
        Task ClaimApprenticeship([Body] ApprenticeshipAssociation association);

        [Get("/apprentices/{id}/apprenticeships")]
        Task<Apprenticeship[]> GetApprenticeships([Path] Guid id);

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

    public interface IObsoleteApi
    {
        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{commitmentStatementId}/trainingproviderconfirmation")]
        [Obsolete]
        Task ConfirmTrainingProvider(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] TrainingProviderConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{commitmentStatementId}/employerconfirmation")]
        [Obsolete]
        Task ConfirmEmployer(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] EmployerConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{commitmentStatementId}/rolesandresponsibilitiesconfirmation")]
        [Obsolete]
        Task ConfirmRolesAndResponsibilities(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] RolesAndResponsibilitiesConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{commitmentStatementId}/apprenticeshipdetailsconfirmation")]
        [Obsolete]
        Task ConfirmApprenticeshipDetails(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] ApprenticeshipDetailsConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/revisions/{commitmentStatementId}/howapprenticeshipwillbedeliveredconfirmation")]
        [Obsolete]
        Task ConfirmHowApprenticeshipDelivered(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] HowApprenticeshipDeliveredConfirmationRequest confirmation);
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