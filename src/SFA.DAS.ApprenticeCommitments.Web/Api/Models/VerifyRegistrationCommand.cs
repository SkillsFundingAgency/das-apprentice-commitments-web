using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Api.Models
{
    public class VerifyRegistrationCommand
    {
        public Guid RegistrationId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NationalInsuranceNumber { get; set; }
    }
}