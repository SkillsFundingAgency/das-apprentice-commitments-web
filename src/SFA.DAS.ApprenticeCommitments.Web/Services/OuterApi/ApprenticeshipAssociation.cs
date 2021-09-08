using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class ApprenticeshipAssociation
    {
        public string RegistrationId { get; set; } = null!;
        public Guid ApprenticeId { get; set; }
    }
}