using RestEase;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Api
{
    public interface ApiClient
    {
        [Get("/registrations/{id}")]
        Task<Registration> GetRegistration([Path] Guid id);
    }
}