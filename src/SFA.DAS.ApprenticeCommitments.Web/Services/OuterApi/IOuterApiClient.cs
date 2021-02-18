using System;
using System.Threading.Tasks;
using RestEase;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public interface IOuterApiClient
    {
        [Get("/registrations/{id}")]
        Task<VerifyRegistrationResponse> GetRegistration([Path] Guid id);

        [Post("/registrations")]
        Task VerifyRegistration([Body] VerifyRegistrationRequest verification);
    }
}