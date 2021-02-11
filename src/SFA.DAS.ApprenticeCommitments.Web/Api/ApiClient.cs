using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public interface IApiClient
    {
        [Get("/registrations/{id}")]
        Task<Registration> GetRegistration([Path] Guid id);

        [Post("/registrations/{id}")]
        Task Validate([Path] Guid id, [Body] VerifyRegistrationCommand verification);
    }
}