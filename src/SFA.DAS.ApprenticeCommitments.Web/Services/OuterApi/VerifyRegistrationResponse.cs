using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class VerifyRegistrationResponse
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
    }
}