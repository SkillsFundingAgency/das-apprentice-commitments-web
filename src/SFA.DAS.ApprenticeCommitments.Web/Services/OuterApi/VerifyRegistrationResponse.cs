using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class VerifyRegistrationResponse
    {
        public Guid ApprenticeId { get; set; }
        public string Email { get; set; }
        public bool HasViewedVerification { get; set; }
        public bool HasCompletedVerification { get; set; }

    }
}