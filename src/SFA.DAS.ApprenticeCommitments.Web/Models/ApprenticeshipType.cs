using System.ComponentModel;

namespace SFA.DAS.ApprenticeCommitments.Web.Models
{
    public enum ApprenticeshipType
    {
        [Description("Apprenticeship")]
        Apprenticeship = 0,
        
        [Description("Foundation apprenticeship")]
        FoundationApprenticeship = 1
    }
}