using RestEase;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient
    {
        [Get("/registrations/{id}")]
        Task<VerifyRegistrationResponse> GetRegistration([Path] Guid id);

        [Post("/registrations")]
        Task VerifyRegistration([Body] VerifyRegistrationRequest verification);

        [Get("/apprentices/{id}/apprenticeships")]
        Task<Apprenticeship[]> GetApprenticeships([Path] Guid id);

        //[Get("/apprentices/{apprenticeid}/apprenticeships/{apprenticeshipid}")]
        //Task<Apprenticeship> GetApprenticeship([Path] Guid apprenticeid, [Path] long apprenticeshipid);
    }
}