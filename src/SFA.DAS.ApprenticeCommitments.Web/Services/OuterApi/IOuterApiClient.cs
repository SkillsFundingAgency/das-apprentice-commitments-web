using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient
    {
        [Get("/registrations/{apprenticeId}")]
        Task<VerifyRegistrationResponse> GetRegistration([Path] Guid apprenticeId);

        [Post("/registrations/{apprenticeId}/firstseen")]
        Task RegistrationFirstSeenOn([Path]Guid apprenticeId, [Body] RegistrationFirstSeenOnRequest request);

        [Post("/registrations")]
        Task VerifyRegistration([Body] VerifyRegistrationRequest verification);

        [Get("/apprentices/{apprenticeId}/apprenticeships")]
        Task<Apprenticeship[]> GetApprenticeships([Path] Guid apprenticeId);

        [Get("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}")]
        Task<Apprenticeship> GetApprenticeship([Path] Guid apprenticeid, [Path] long apprenticeshipid);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/trainingproviderconfirmation")]
        Task ConfirmTrainingProvider(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid,
            [Body] TrainingProviderConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/employerconfirmation")]
        Task ConfirmEmployer(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid,
            [Body] EmployerConfirmationRequest confirmation);

        [Post("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}/apprenticeshipdetailsconfirmation")]
        Task ConfirmApprenticeshipDetails(
            [Path] Guid apprenticeid, [Path] long apprenticeshipid,
            [Body] ApprenticeshipDetailsConfirmationRequest confirmation);

    }
}