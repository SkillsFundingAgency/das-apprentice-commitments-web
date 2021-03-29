using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class VerifyRegistrationRequest
    {
        public Guid ApprenticeId { get; set; }
        public Guid UserIdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NationalInsuranceNumber { get; set; }
    }
}