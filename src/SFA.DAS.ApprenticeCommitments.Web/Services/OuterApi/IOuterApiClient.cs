using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient
    {
        [Get("/registrations/{id}")]
        Task<VerifyRegistrationResponse> GetRegistration([Path] Guid id);

        [Post("/registrations/{apprenticeId}/firstseen")]
        Task RegistrationFirstSeenOn([Path] Guid apprenticeId, [Body] RegistrationFirstSeenOnRequest request);

        [Post("/registrations")]
        Task VerifyRegistration([Body] VerifyRegistrationRequest verification);

        [Get("/apprentices/{id}/apprenticeships")]
        Task<Apprenticeship[]> GetApprenticeships([Path] Guid id);

        [Get("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}")]
        Task<Apprenticeship> GetApprenticeship([Path] Guid apprenticeid, [Path] long apprenticeshipid);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/trainingproviderconfirmation")]
        Task ConfirmTrainingProvider(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] TrainingProviderConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/employerconfirmation")]
        Task ConfirmEmployer(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] EmployerConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/rolesandresponsibilitiesconfirmation")]
        Task ConfirmRolesAndResponsibilities(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] RolesAndResponsibilitiesConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/apprenticeshipdetailsconfirmation")]
        Task ConfirmApprenticeshipDetails(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] ApprenticeshipDetailsConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/howapprenticeshipwillbedeliveredconfirmation")]
        Task ConfirmHowApprenticeshipDelivered(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] HowApprenticeshipDeliveredConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/statements/{commitmentStatementId}/apprenticeshipconfirmation")]
        Task ConfirmApprenticeship(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid, [Path] long commitmentStatementId,
            [Body] ApprenticeshipConfirmationRequest confirmation);
    }
}