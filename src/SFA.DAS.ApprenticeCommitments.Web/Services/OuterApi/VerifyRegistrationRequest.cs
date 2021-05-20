using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class VerifyRegistrationRequest
    {
        public Guid ApprenticeId { get; set; }
        public Guid UserIdentityId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string NationalInsuranceNumber { get; set; } = null!;
    }
}